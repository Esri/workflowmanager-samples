import argparse
import logging
from math import ceil


def main():
    parsed_args = parse_arguments(None)

    (jobs_table, job_progress_table) = connect(parsed_args)
    delete_jobs(parsed_args, jobs_table, job_progress_table)


def parse_arguments(args):
    parser = argparse.ArgumentParser(
        description='This utility is used to delete old jobs from the specified ArcGIS Workflow Manager item. '
                    'This must be run by the owner of the Workflow Item in order to ensure write access to the '
                    'underlying Workflow Manager schema feature layers'
    )
    parser.add_argument('portalUrl', help='Portal URL. Example: https://server.domain.com/webadaptor')
    parser.add_argument('itemId', help='Workflow Manager item ID')
    group = parser.add_argument_group('Connection Options')
    group.add_argument('-u', '--username', help='Username. At least one of username or profile must be specified')
    group.add_argument('-p', '--password', help='Password. At least one of password or profile must be specified')
    group.add_argument('--profile', help='Profile name for stored credentials. '
                                         'At least one of username/password or profile must be specified')

    parser.add_argument('-s', '--startAge', type=int, default=-1,
                        help='Only delete records from jobs that have been closed less than this number of '
                             'days (must be between -1 and 365 and > end-age). If -1, that means records are deleted '
                             'for all jobs older than endAge')

    parser.add_argument('-e', '--endAge', type=int, default=7,
                        help='Only delete records from jobs that have been closed at least this number of '
                             'days (must be between 0 and 365 and < start-age)')

    parser.add_argument('-v', '--verbose',
                        action='count',
                        dest='verbosity',
                        default=0,
                        help="Verbose output (repeat for increased verbosity)")
    parser.add_argument('-q', '--quiet',
                        action='store_const',
                        const=-1,
                        default=0,
                        dest='verbosity',
                        help="Quiet output (show errors only)")

    parsed_args = parser.parse_args(args)

    if parsed_args.startAge > 365 or parsed_args.startAge < -1:
        parser.error('Start Age must be between -1 and 365')

    if parsed_args.endAge > 365 or parsed_args.endAge < 0:
        parser.error('End Age must be between 0 and 365')

    if not (parsed_args.startAge < 0 or parsed_args.startAge > parsed_args.endAge):
        parser.error('Start Age must be > End Age')

    if not parsed_args.username and not parsed_args.profile:
        parser.error('At least one of username or profile must be specified')

    if not parsed_args.password and not parsed_args.profile:
        parser.error('At least one of password or profile must be specified')

    return parsed_args


def setup_logging(verbosity):
    # From https://xahteiwi.eu/resources/hints-and-kinks/python-cli-logging-options/
    base_loglevel = 30
    verbosity = min(verbosity, 2)
    loglevel = base_loglevel - (verbosity * 10)
    logging.basicConfig(level=loglevel)


def connect(args):
    # Wait to import these until after parsing the arguments to make -h and failure cases much faster
    import arcgis

    # Do this after importing arcgis as at higher log levels it will log some info we don't care about
    setup_logging(args.verbosity)

    logging.info('Connecting')
    gis = arcgis.gis.GIS(args.portalUrl, username=args.username, password=args.password, profile=args.profile)
    item = gis.content.get(args.itemId)
    if not item:
        raise RuntimeError(f'Item {args.itemId} does not exist or is not shared with the user')
    else:
        logging.info(f'Cleaning up item "{item.title}"')

    # Get tables
    item_title = f'workflow_{args.itemId}'
    search_results = [x for x in gis.content.search(f'title:"{item_title}", type:"Feature Service"')
                      if x['title'] == item_title]
    if not(len(search_results) == 1):
        raise RuntimeError(f'Unable to determine Workflow schema layer.')
    workflow_schema = search_results[0]

    table_results = [x for x in workflow_schema.tables if x.properties['name'] == 'jobs']
    if not (len(table_results) == 1):
        raise RuntimeError('Unable to determine Jobs table')
    jobs_table = table_results[0]

    table_results = [x for x in workflow_schema.tables if x.properties['name'] == 'jobProgress']
    if not (len(table_results) == 1):
        raise RuntimeError('Unable to determine Job Progress table')
    job_progress_table = table_results[0]

    return jobs_table, job_progress_table


def delete_jobs(args, jobs_table, job_progress_table):
    from timeit import default_timer as timer
    from datetime import datetime, timedelta
    import numpy

    now = datetime.utcnow()
    min_closed = (now - timedelta(days=args.startAge)).isoformat()
    max_closed = (now - timedelta(days=args.endAge)).isoformat()

    start = timer()
    successful = True
    if args.startAge > 0:
        query = f"closed = 1 AND end_date > '{min_closed}' AND end_date <= '{max_closed}'"
    else:
        query = f"closed = 1 AND end_date <= '{max_closed}'"

    logging.info(f'Deleting existing jobs where "{query}"')

    search_results = jobs_table.query(where=query, out_fields=['job_id'], return_geometry=False)
    # Handle the case where then are 0 right off the start
    if not search_results.features:
        logging.info('No jobs requiring cleanup')
        return

    # Delete in batches of 1000 job ids
    for chunk in numpy.array_split(search_results.features, ceil(len(search_results.features) / 1000)):
        job_ids = [f"'{x.get_value('job_id')}'" for x in chunk]
        logging.info(f'Deleting job progress from {len(job_ids)} jobs')

        where = f"job_id in ({','.join(job_ids)})"
        logging.debug(f'Deleting job progress where {where}')
        result = job_progress_table.delete_features(where=where, rollback_on_failure=False, return_delete_results=False)
        logging.info(f'Result = {result}')
        # Results are different in different versions of Python Api for ArcGIS, so handle both types of results
        if not ('success' in result and result['success']) and \
                not ('deleteResults' in result and all(x['success'] for x in result['deleteResults'])):
            successful = False

    end = timer()
    logging.info(f'Deletion completed in {round(end - start, 2)}s')
    if not successful:
        logging.error("Failed to delete all job progress records")
        exit(2)


if __name__ == '__main__':
    main()
