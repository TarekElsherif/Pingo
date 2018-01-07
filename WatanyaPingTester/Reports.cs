﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatanyaPingTester
{   
    class Reports
    {
        Microsoft.Office.Interop.Excel.Application oXL;
        Microsoft.Office.Interop.Excel._Workbook oWB;
        Microsoft.Office.Interop.Excel._Worksheet oSheet;
        Microsoft.Office.Interop.Excel.Range oRng;
        object misvalue = System.Reflection.Missing.Value;
        
        // nodeDataInfo.nodePathData 
        /*        0                        1            2                 3
         * 0     Index In Scheme        NodeName       IP        OnlinePercentage 
         * 1     Index In Scheme        NodeName       IP        OnlinePercentage
         */
        List<SchemeNode> schemeNodes;
        string resPath;
        List<List<string>> collectingPorts;
        NodePathFromCompany nodePathFromCompany;
        List<NodePathInfo> nodePathInfoList = new List<NodePathInfo>();

        public Reports(string resPath, List<List<string>> collectingPorts, List<SchemeNode> schemeNodes)
        {
            this.collectingPorts = collectingPorts;
            this.schemeNodes = schemeNodes;
            this.resPath = resPath;
            fillNodePathInfoList();
            createColorScaleExcel();
        }

        public void fillNodePathInfoList(){
            for (int i = 0; i < collectingPorts.Count(); i++ )
            {
                nodePathFromCompany = new NodePathFromCompany(this.collectingPorts[i], this.schemeNodes);
                nodePathFromCompany.createNodePath();
                nodePathInfoList.Add(nodePathFromCompany.getNodePathInfo());
                for (int j = 0; j < nodePathInfoList[i].nodePathData.Count(); j++)
                {
                    int nodeIndex = Int32.Parse(nodePathInfoList[i].nodePathData[j].ElementAt(0));
                    nodePathInfoList[i].nodePathData[j].Add(getNodeStatus(nodeIndex).ToString());
                }
            }
        }

        private double getNodeStatus(int nodeIndex)
        {
            double onlinePerc = 0.00, overallPing;
            overallPing = schemeNodes[nodeIndex].getOnlineCount() + schemeNodes[nodeIndex].getOfflineCount() + schemeNodes[nodeIndex].getTimeoutCount();
            onlinePerc = (schemeNodes[nodeIndex].getOnlineCount() / overallPing) * 100;
            return Math.Round(onlinePerc, 2);
        }

        public void createColorScaleExcel()
        {
            try
            {
                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                
                //Format A1:E1 as bold, vertical alignment = center.
                //oSheet.get_Range("A1", "E1").Font.Bold = true;
                //oSheet.get_Range("A1", "E1").VerticalAlignment =
                //Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                int n = 1, m = 1;
                for (int i = 0; i < nodePathInfoList.Count(); i++)
                {
                    m = i + m;
                    oSheet.Cells[m, 1].Value2 = nodePathInfoList[i].portName;
                    for (int j = 0; j < nodePathInfoList[i].nodePathData.Count(); j++)
                    {
                        oSheet.Cells[m + 1, j + 1].Value2 = nodePathInfoList[i].nodePathData[j].ElementAt(1);
                        oSheet.Cells[m + 2, j + 1].Value2 = nodePathInfoList[i].nodePathData[j].ElementAt(2);
                        oSheet.Cells[m + 3, j + 1].Value2 = nodePathInfoList[i].nodePathData[j].ElementAt(3);
                    }
                    n++;
                    m += 4;
                }

                oRng = oSheet.get_Range("A1", "E1");
                oRng.EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(resPath + "\\sokhnaColorReport.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();

            }
            catch (Exception e)
            {

            }
        }

        //public void createChartTable()
        //{
        //    try
        //    {
        //        //Start Excel and get Application object.
        //        oXL = new Microsoft.Office.Interop.Excel.Application();
        //        oXL.Visible = true;

        //        //Get a new workbook.
        //        oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
        //        oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

        //        //Add table headers going cell by cell.
        //        oSheet.Cells[1, 1] = "Name";
        //        oSheet.Cells[1, 2] = "IP";
        //        oSheet.Cells[1, 3] = "Online";
        //        oSheet.Cells[1, 4] = "Not Reachable";
        //        oSheet.Cells[1, 5] = "Timeout";

        //        //Format A1:E1 as bold, vertical alignment = center.
        //        oSheet.get_Range("A1", "E1").Font.Bold = true;
        //        oSheet.get_Range("A1", "E1").VerticalAlignment =
        //        Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

        //        for (int i = 0; i < nodePathData.Count(); i++)
        //        {
        //            for (int j = 0; j < nodePathData[0].Count(); j++)
        //            {
        //                oSheet.Cells[i + 2, j + 1].Value2 = nodePathData[i].ElementAt(j);
        //            }
        //        }
        //        oRng = oSheet.get_Range("A1", "E1");
        //        oRng.EntireColumn.AutoFit();

        //        oXL.Visible = false;
        //        oXL.UserControl = false;
        //        oWB.SaveAs(resPath + "\\sokhnaReport.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
        //            false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
        //            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        //        oWB.Close();

        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
    }

    public class NodePathInfo
    {
        public List<List<string>> nodePathData = new List<List<string>>();
        public string portName;
    }
}