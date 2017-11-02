using CrystalDecisions.CrystalReports.Engine;
using DMS.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.Models;
using DMS.BL;
using System.Data;

namespace DMS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewOpenDefect(Report rpt)
        {
            DataTable notClosed_DT = CLASS_REPORT.SP_TOTAL_NOT_CLOSED_DEFECT(rpt.projectID);
            DataTable Total_Defect_DT = CLASS_REPORT.SP_TOTAL_DEFECT(rpt.projectID);

            int TotalNotClosed = notClosed_DT.Rows.Count;
            int TotalDefect = Total_Defect_DT.Rows.Count;

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "OpenDefect_RPT.rpt"));

            rd.SetDataSource(CLASS_REPORT.getDataforOpenVSclosed(rpt.projectID));
            rd.SetParameterValue("total_closed", TotalNotClosed);
            rd.SetParameterValue("total_defect", TotalDefect);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_ViewOpenDefctandClosedDef.pdf");
        }
         
        public ActionResult ViewDefectDensity_RPT(Report rpt)
        {
            DataTable Total_Defect_DT = CLASS_REPORT.SP_GET_DEFECT_DEFECT_DENSITY_MODUEL_WISE(rpt.projectID,rpt.MID);
            DataTable Total_TESTCASE_DT = CLASS_REPORT.SP_GET_TOTAL_TESTCASE_MODULE_WISE(rpt.projectID,rpt.MID);

            int TotalDefect = Total_Defect_DT.Rows.Count;
            int TotalTestCase = Total_TESTCASE_DT.Rows.Count;


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "DEFECT_DENSITY.rpt"));

            rd.SetDataSource(CLASS_REPORT.SP_GET_DEFECT_DEFECT_DENSITY_MODUEL_WISE(rpt.projectID,rpt.MID));
            rd.SetParameterValue("TotalDefect", TotalDefect);
            rd.SetParameterValue("TotalTestCase", TotalTestCase);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_Defect_Dencity.pdf");
        }
         
         
        public ActionResult DRR(Report rpt)
        {
            string projectID = rpt.projectID;
            int QA_ID = rpt.QA_ID , TotalRaisedDefect=0,TotalRejectDefect=0;

            DataTable Defect_DT = CLASS_REPORT.SP_TOTAL_RAISED_DEFEECT(projectID, "DEFECT", QA_ID);
            DataTable Reject_DT = CLASS_REPORT.SP_TOTAL_RAISED_DEFEECT(projectID, "REJECT", QA_ID);
            
            // get total count 
            TotalRaisedDefect = Defect_DT.Rows.Count;
            TotalRejectDefect = Reject_DT.Rows.Count;


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "DRR.rpt")); // Report location

            rd.SetDataSource(CLASS_REPORT.getDataforDRR(projectID,QA_ID));
            rd.SetParameterValue("TOTAL_RAISED", TotalRaisedDefect);
            rd.SetParameterValue("TOTAL_REJECT", TotalRejectDefect);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_DRR.pdf");
        }


        public ActionResult LifeCycle(Report rpt)
        {
            DataTable Total_OTHERS_DT = CLASS_REPORT.SP_GET_DEFECT_OTHERS(rpt.projectID);
            DataTable Total_DESIGNE_DT = CLASS_REPORT.SP_GET_DEFECT_DESIGN(rpt.projectID);
            DataTable Total_REQUIREMNT_DT = CLASS_REPORT.SP_GET_DEFECT_REQUIRMENT(rpt.projectID);
            DataTable Total_DEVELOPM_DT = CLASS_REPORT.SP_GET_DEFECT_DEVELOPMENT(rpt.projectID);

            int OTHERS = Total_OTHERS_DT.Rows.Count;
            int DESIGN = Total_DESIGNE_DT.Rows.Count;
            int DEVELOPMNT = Total_DEVELOPM_DT.Rows.Count;
            int REQUIREMNT = Total_REQUIREMNT_DT.Rows.Count;


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "PRO_LIFE_CYCLE.rpt")); // report location

            rd.SetDataSource(CLASS_REPORT.getDataforLifeCycle(rpt.projectID));
            rd.SetParameterValue("design", DESIGN);
            rd.SetParameterValue("requirenment", REQUIREMNT);
            rd.SetParameterValue("developemnt", DEVELOPMNT);
            rd.SetParameterValue("others", OTHERS);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_Defect_Life_Cycle.pdf");
        }

        public ActionResult DefectLeakage(Report rpt)
        {
            DataTable Total_Defect_DT = CLASS_REPORT.SP_TOTAL_DEFECT(rpt.projectID);

            int clientIdentifiedDefects =rpt.ClientIdendtifyDefect;
            int QA_defect = Total_Defect_DT.Rows.Count;

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "DefectLeakage.rpt")); // report location

            rd.SetDataSource(CLASS_REPORT.getDataforLifeCycle(rpt.projectID));
            rd.SetParameterValue("clientIdentifiedDefects", clientIdentifiedDefects);
            rd.SetParameterValue("QA_defect", QA_defect);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_DefectLeakage.pdf");
        }

        public ActionResult Defectagingreport(Report rpt)
        {

            string projectID = rpt.projectID, defectWeak="";
            int DefectAge = rpt.weak, TotalRaisedDefect = 0;

            switch (DefectAge)
            {
                case -7:
                    defectWeak = "1 Weak (7 days)";
                    break;
                case -14:
                    defectWeak = "2 Weak (14 days)";

                    break;
                case -21:
                    defectWeak = "3 Weak (21 days)";

                    break;
                case -28:
                    defectWeak = "4 Weak (28 days)";

                    break;
                case -100:
                    defectWeak = "More than 4 Weak (100 days)";

                    break;
                default:
                    break;
            }

            DataTable Defect_DT = CLASS_REPORT.SP_GET_DEFECT_AGE(projectID, DefectAge);
           // DataTable Reject_DT = CLASS_REPORT.SP_TOTAL_RAISED_DEFEECT(projectID, "REJECT", QA_ID);

            // get total count 
            TotalRaisedDefect = Defect_DT.Rows.Count;
           // TotalRejectDefect = Reject_DT.Rows.Count;


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "DefectAgingReport.rpt")); // Report location

             rd.SetDataSource(CLASS_REPORT.getDataforDefectAging(projectID,DefectAge));
             rd.SetParameterValue("TOTAL_RAISED", TotalRaisedDefect);
              rd.SetParameterValue("DefectAge", defectWeak);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_DefectAgingRepor.pdf");
        }

        public ActionResult DefectSeverity(Report rpt)
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "DefectSeverity.rpt")); // report location

            rd.SetDataSource(CLASS_REPORT.getDataforDefectSeverity(rpt.projectID));
            

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_DefectSeverity.pdf");

        }

        public ActionResult RCA(Report rpt)
        {
            int submiting_defects = 0, AssigningDefect = 0, fixingDefects = 0, closingDefects = 0;
            string rca = rpt.RCA;

            DataTable closedDefect_DT = CLASS_REPORT.SP_GET_DEFECT_RCA(rpt.projectID, "CLOSED", rpt.RCA);
            DataTable submiting_defectsDefect_DT = CLASS_REPORT.SP_GET_DEFECT_RCA(rpt.projectID, "RAISED", rpt.RCA);
            DataTable fixingDefects_DT = CLASS_REPORT.SP_GET_DEFECT_RCA(rpt.projectID, "FIXED", rpt.RCA);
            DataTable submitingDefects_DT = CLASS_REPORT.SP_GET_DEFECT_RCA(rpt.projectID, "ASSiGNED TO DEVELOPER", rpt.RCA);

            submiting_defects = submiting_defectsDefect_DT.Rows.Count;
            AssigningDefect = submitingDefects_DT.Rows.Count;
            fixingDefects = fixingDefects_DT.Rows.Count;
            closingDefects = submitingDefects_DT.Rows.Count;


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/RPT"), "RCA.rpt")); // report location

            rd.SetDataSource(CLASS_REPORT.getDefectRCA(rpt.projectID,rpt.RCA));
            rd.SetParameterValue("submiting_defects", submiting_defects);
            rd.SetParameterValue("AssigningDefect", AssigningDefect);
            rd.SetParameterValue("fixingDefects", fixingDefects);
            rd.SetParameterValue("closingDefects", closingDefects);
            rd.SetParameterValue("rca", rca);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", rpt.projectID + "_RCA.pdf");

        }

    }
}