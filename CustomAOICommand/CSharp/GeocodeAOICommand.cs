using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.JTXUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace JTXSamples
{
    public class GeocodeAOICommand : BaseCommand, IJTXAOICommand, IJTXXMLConfiguration
    {
        private IHookHelper m_hookHelper = null;
        private IJTXAOIPanel m_pAOIPanel = null;

        private string m_strWorkspace;
        private string m_strLocator;

        public GeocodeAOICommand()
        {
            base.m_category = ""; //localizable text
            base.m_caption = "Find Address";  //localizable text 
            base.m_message = "Find Address";  //localizable text
            base.m_toolTip = "Find Address";  //localizable text
            base.m_name = "JTXCustomCommands.GeocodeAOICommand";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            base.m_bitmap = Properties.Resources.GeocodeAOICommandIcon;
        }


        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;
        }

        public override void OnClick()
        {
            if (m_pAOIPanel.CurrentJob != null)
            {               
                //show form
                AddressDialog pAddressDialog = new AddressDialog();

                if (pAddressDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {       
                    ILocatorManager pLocatorMgr = new LocatorManagerClass();
                    ILocatorWorkspace pLocatorWS = pLocatorMgr.GetLocatorWorkspaceFromPath(m_strWorkspace);
                    
                    IAddressGeocoding pLocator = (IAddressGeocoding)pLocatorWS.GetLocator(m_strLocator);

                    IPropertySet addressProperties = new PropertySetClass();
                    addressProperties.SetProperty("Street", pAddressDialog.street);

                    IPropertySet matchProperties = pLocator.MatchAddress(addressProperties);

                    if(pLocator.MatchFields.FieldCount == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("No address found");
                        return;
                    }

                    IPoint pPoint = null;
                    for(int i = 0; i < pLocator.MatchFields.FieldCount; i++)
                    {
                        if(pLocator.MatchFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                        {
                            object pObject = matchProperties.GetProperty(pLocator.MatchFields.get_Field(i).Name);
                            if(pObject is IPoint)
                                pPoint = (IPoint) pObject;
                        }
                    }
                    //calculate AOI

                    ITopologicalOperator pTopo = (ITopologicalOperator)pPoint;
                    IGeometry pGeom = pTopo.Buffer(100);

                    IEnvelope pMyAOI = pGeom.Envelope;

                    m_pAOIPanel.CurrentAOI = CreatePolyFromEnv(pMyAOI);
                    IEnvelope pEnv = pGeom.Envelope;
                    pEnv.Expand(2, 2, true);
                    m_hookHelper.ActiveView.Extent = pEnv;
                    m_hookHelper.ActiveView.Refresh();
                    
                }
            }

        }

        public override bool Enabled
        {
            get
            {
                return m_pAOIPanel.CurrentJob != null;
            }
        }


        #region IJTXAOICommand Members

        public void Attach(IJTXAOIPanel pAOIPanel)
        {
            m_pAOIPanel = pAOIPanel;
        }

        #endregion

        public static IPolygon CreatePolyFromEnv(IEnvelope pEnv)
        {
            if (pEnv == null)
                return null;
            object o = Type.Missing;

            IPointCollection pPointCol = new PolygonClass();

            IPoint pPoint = new PointClass();
            pPoint.PutCoords(pEnv.XMin, pEnv.YMin);
            pPointCol.AddPoint(pPoint, ref o, ref o);

            pPoint = new PointClass();
            pPoint.PutCoords(pEnv.XMin, pEnv.YMax);
            pPointCol.AddPoint(pPoint, ref o, ref o);

            pPoint = new PointClass();
            pPoint.PutCoords(pEnv.XMax, pEnv.YMax);
            pPointCol.AddPoint(pPoint, ref o, ref o);

            pPoint = new PointClass();
            pPoint.PutCoords(pEnv.XMax, pEnv.YMin);
            pPointCol.AddPoint(pPoint, ref o, ref o);

            pPoint = new PointClass();
            pPoint.PutCoords(pEnv.XMin, pEnv.YMin);
            pPointCol.AddPoint(pPoint, ref o, ref o);

            IGeometry pGeom = (IGeometry)pPointCol;
            pGeom.SpatialReference = pEnv.SpatialReference;

            return (IPolygon)pGeom;
        }


        #region IJTXXMLConfiguration Members

        public string ConfigurationXML
        {
            set 
            {
                System.IO.Stream xmlStream = ConvertStringToStream(value);

                System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(xmlStream);

                System.Xml.XPath.XPathDocument xpathDoc = new System.Xml.XPath.XPathDocument(xmlReader);
                System.Xml.XPath.XPathNavigator xpathNav = xpathDoc.CreateNavigator();
                System.Xml.XPath.XPathNodeIterator xpathNode = xpathNav.Select("//PROPERTIES");

                if(xpathNode.MoveNext())
                {
                    System.Xml.XPath.XPathNavigator xpathCurrentNode = xpathNode.Current;

                    m_strWorkspace = xpathCurrentNode.GetAttribute("workspace", "");
                    m_strLocator = xpathCurrentNode.GetAttribute("locator", "");
                    
                }
            }
        }

        private static System.IO.Stream ConvertStringToStream(string str)
        {
            int length = str.Length;
            byte[] buffer = new byte[length];

            for (int index = 0; index < length; ++index)
            {
                buffer[index] = (byte)str[index];
            }

            return new System.IO.MemoryStream(buffer);
        }

        #endregion
    }
}
