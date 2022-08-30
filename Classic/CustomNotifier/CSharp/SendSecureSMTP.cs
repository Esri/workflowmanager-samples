/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.​*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.esriSystem;
using Microsoft.Win32;
using System.Net;
using System.Net.Mail;

namespace JTXSamples
{
    [Guid("78400FF9-5C70-47c3-85FA-ADB466D5BB6E")]
    public class SendSecureSMTP : IJTXMailNotifier2, IJTXNotifier, ILogSupport
    {
        private ILog m_ipLog = null;
        private String m_strSMTPServer = null;
        private String m_strSMTPUser = null;
        private String m_strSMTPPassword = null;
        private bool m_bHTMLSupport = true;
        private String m_strSMTPPort = null;

        #region ILogSupport Members
        public void InitLogging(ILog ipLog)
        {
            m_ipLog = ipLog;
        }
        private void LogMessage(int Level, int Code, String strMessage)
        {
            if (m_ipLog != null)
            {
                m_ipLog.AddMessage(Level, Code, strMessage);
            }
        }
        #endregion

        #region IJTXMailNotifier Members

        public bool HTMLSupport
        {
            get
            {
                return m_bHTMLSupport;
            }
            set
            {
                m_bHTMLSupport = value;
            }
        }

        public string SMTPServer
        {
            get
            {
                return m_strSMTPServer;
            }
            set
            {
                m_strSMTPServer = value;
            }
        }

	    #endregion

        #region IJTXMailNotifier2

        public string SMTPPassword
        {
            get
            {
                return m_strSMTPPassword;
            }
            set
            {
                m_strSMTPPassword = value;
            }
        }
    
        

        public string SMTPUser
        {
            get
            {
                return m_strSMTPUser;
            }
            set
            {
                m_strSMTPUser = value;
            }
        }

        public string SMTPPort
        {
            get
            {
                return m_strSMTPPort;
            }
            set
            {
                m_strSMTPPort = value;
            }
        }

        // These are ignored in this sample
        public string FileToAttach { get; set; }
        public int MaxAttachmentSize { get; set; }
        public string Protocol { get; set; }

        #endregion

        #region IJTXNotifier Members

        public void Send(IJTXNotification ipNotification)
        {
            try
            {
                LogMessage(5, 1000, "SendSecureSMTP: Setting up to send a secure notification to " + m_strSMTPServer + ", on Port " + m_strSMTPPort);
                SmtpClient mailClient = new SmtpClient(m_strSMTPServer);

                ////////////////////////////////////////
                // NOTE: This is for sample purposes only... Should be an encrypted password that must be descrypted first
                ////////////////////////////////////////

                ICredentialsByHost ipCredentials = new NetworkCredential(m_strSMTPUser, m_strSMTPPassword);
                mailClient.EnableSsl = true;
                mailClient.Credentials = ipCredentials;
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Host = m_strSMTPServer;
                int smtpPort = 25;
                if (Int32.TryParse(m_strSMTPPort, out smtpPort) == false)
                {
                    smtpPort = 25;
                }
                mailClient.Port = smtpPort;
                mailClient.Timeout = 10000;
                LogMessage(5, 1000, "SendSecureSMTP: Finished setting up the credentials necessary for the secure notification..");

                string[] arrRecipients = ipNotification.get_Recipients();
                for (int i = 0; i < arrRecipients.Length; ++i)
                {
                    // Parse the recipient list to determine if they contain comma or semi-colon delimited
                    // lists of individual recipients
                    arrRecipients[i] = arrRecipients[i].Replace(',', ';');
                    string[] subRecipients = arrRecipients[i].Split(';');
                    foreach (string recipient in subRecipients)
                    {
                        MailMessage mailMessage;
                        try
                        {
                            LogMessage(5, 1000, "SendSecureSMTP: Sending email from: " + ipNotification.SenderEmail + ", with subject: " + ipNotification.Subject + ", to: " + arrRecipients[i]);
                            MailAddress From = new MailAddress(ipNotification.SenderEmail, ipNotification.SenderDisplayName);
                            MailAddress To = new MailAddress(arrRecipients[i]);
                            mailMessage = new MailMessage(From, To);
                            mailMessage.Sender = new MailAddress(ipNotification.SenderEmail, ipNotification.SenderDisplayName);
                            mailMessage.Subject = ipNotification.Subject;
                            mailMessage.Body = ipNotification.MessageBody;
                            mailMessage.IsBodyHtml = m_bHTMLSupport;
                        }
                        catch (Exception ex)
                        {
                            String strMessage = ex.Message;
                            Exception innerEx = ex.InnerException;
                            while (innerEx != null)
                            {
                                // Build the error message containing all the inner exceptions, as they are used by the mail client
                                strMessage += " -> " + innerEx.Message;
                                innerEx = innerEx.InnerException;
                            }
                            LogMessage(5, 1000, "SendSecureSMTP: Caught an exception: " + strMessage + ", continuing onto the next recipient.. ");

                            continue;
                        }
                        mailClient.EnableSsl = true;
                        mailClient.Send(mailMessage);
                        LogMessage(5, 1000, "SendSecureSMTP: Finished sending email from: " + ipNotification.SenderEmail + ", with subject: " + ipNotification.Subject + ", to: " + arrRecipients[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                String strMessage = ex.Message;
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    // Build the error message containing all the inner exceptions, as they are used by the mail client
                    strMessage += " -> " + innerEx.Message;
                    innerEx = innerEx.InnerException;
                }

                LogMessage(5, 1000, "SendSecureSMTP: Caught an exception: " + strMessage);
            }
        }

        #endregion
    }
}
