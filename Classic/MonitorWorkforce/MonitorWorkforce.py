import atexit
import configparser
import sqlite3
import logging
import logging.handlers
import sys
import datetime
import time
try:    
    from urlparse import urljoin
except ImportError:
    from urllib.parse import urljoin
from json import JSONDecodeError
from arcgis.apps import workforce
from arcgis.gis import GIS
import requests


class WorkflowServerError(RuntimeError):
    def __init__(self, msg):
        self.msg = msg


def initialize_logging(log_file):
    """
    Setup logging
    :param log_file: (string) The file to log to
    :return: (Logger) a logging instance
    """
    # initialize logging
    formatter = logging.Formatter("[%(asctime)s] [%(filename)30s:%(lineno)4s - %(funcName)30s()]\
             [%(threadName)5s] [%(name)10.10s] [%(levelname)8s] %(message)s")
    # Grab the root logger
    logger = logging.getLogger()
    # Set the root logger logging level (DEBUG, INFO, WARNING, ERROR, CRITICAL)
    logger.setLevel(logging.DEBUG)
    # Create a handler to print to the console
    sh = logging.StreamHandler(sys.stdout)
    sh.setFormatter(formatter)
    sh.setLevel(logging.INFO)
    # Create a handler to log to the specified file
    rh = logging.handlers.RotatingFileHandler(log_file, mode='a', maxBytes=10485760)
    rh.setFormatter(formatter)
    rh.setLevel(logging.DEBUG)
    # Add the handlers to the root logger
    logger.addHandler(sh)
    logger.addHandler(rh)
    return logger


def initialize_db(db):
    """
    Initializes the database and creates the table if necessary
    :param db: (string) The database to use
    :return:
    """
    conn = sqlite3.connect(db)
    c = conn.cursor()
    c.execute("CREATE TABLE IF NOT EXISTS `Assignments` ( "
              "`GlobalID` TEXT UNIQUE, "
              "PRIMARY KEY(`GlobalID`) )")
    conn.commit()
    return conn


def add_assignment_to_db(db_conn, assignment):
    """
    Adds an assignment to the database
    :param db: (string) The database to connect to
    :param assignment: (Feature) The assignment to add
    :return:
    """
    c = db_conn.cursor()
    params = (
        assignment.global_id,
    )
    c.execute("INSERT INTO assignments VALUES (?)",
              params)


def is_assignment_processed(db_conn, assignment):
    """
    Gets all global ids in the database
    :param db: (string) The database to query
    :return: List<string> The global ids in the db
    """
    c = db_conn.cursor()
    c.execute("SELECT GlobalID from assignments where GlobalID=?", (assignment.global_id,))
    global_ids = [r[0] for r in c.fetchall()]
    return bool(global_ids)


def process_assignment(assignment, db_conn, config, logger):
    logger.info("Assignment {} is associated with job {}"
                .format(assignment.id, assignment.work_order_id))
    params = {'f': 'json',
              'user': config["WORKFLOWMANAGER"]["USER"]}
    execute_url_fragment = "jobs/{}/workflow/steps/current/markAsDone".format(assignment.work_order_id)
    url = urljoin(config["WORKFLOWMANAGER"]["URL"], execute_url_fragment)
    logger.debug("Completing current step for {} - {}".format(assignment.work_order_id, url))
    try:
        with db_conn:
            result = requests.get(url, params=params, verify=config["WORKFLOWMANAGER"].getboolean("VERIFY"))
            if result.status_code >= 300:
                raise WorkflowServerError('Failed to complete step. Result is {} - {}'
                                          .format(result.status_code, result.text))
            # ArcGIS Server / Workflow Manager Server don't always return their errors with proper status codes,
            # so check for other types of errors
            try:
                json = result.json()
                if 'error' in json:
                    raise WorkflowServerError('Failed to complete step. Result is {}'.format(json['error']))
            except JSONDecodeError:
                raise WorkflowServerError('Failed to complete step. Result is {}'.format(result.text))

            logger.debug('Result is {} - {}'.format(result.status_code, result.text))
    except WorkflowServerError as e:
        logger.error(e.msg)


def close(db_conn):
    print('Disconnecting from DB')
    db_conn.close()


def main():
    config = configparser.ConfigParser()
    config.read("config.ini")

    logger = initialize_logging(config["LOG"]["LOGFILE"])
    db_conn = initialize_db(config["DB"]["DATABASE"])
    atexit.register(close, db_conn)

    logger.info("Authenticating with ArcGIS Online...")
    gis = GIS(username=config["AGOL"]["USERNAME"],
              password=config["AGOL"]["PASSWORD"])

    logger.info("Getting project info...")
    project = workforce.Project(gis.content.get(config["WORKFORCE"]["PROJECT"]))

    while True:
        logger.info("Querying assignments...")
        timestamp_last_hour = (datetime.datetime.utcnow() - datetime.timedelta(hours=1)).strftime(
            "%Y-%m-%d %H:%M:%S")

        # If your Portal/Organization does not support Simplified Queries, this query may not work
        # In that event, you can use the subsequent one, but this will not stop trying old assignments that have failed
        query = "{} = 3 AND {} >= timestamp '{}' AND {} IS NOT NULL".format(
            project._assignment_schema.status,
            project._assignment_schema.completed_date,
            timestamp_last_hour,
            project._assignment_schema.work_order_id
        )
        # query = "{} = 3 AND {} IS NOT NULL".format(
        #     project._assignment_schema.status,
        #     project._assignment_schema.work_order_id
        # )
        logger.debug("Query is " + query)
        assignments = project.assignments.search(query)

        logger.info("Processing assignments...")
        for assignment in assignments:
            if not is_assignment_processed(db_conn, assignment):
                add_assignment_to_db(db_conn, assignment)
                # Do work here
                process_assignment(assignment, db_conn, config, logger)

        logger.info("Sleeping for 5 seconds...")
        time.sleep(5)


if __name__ == "__main__":
    main()
