﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.AppManager;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;


namespace TheTechIdea.Beep.TreeNodes.Reports
{
    [AddinAttribute(Caption = "Reports", BranchType = EnumPointType.Category, Name = "ReportCategoryNode.Beep", misc = "Beep", iconimage = "category.png", menu = "Beep", ObjectType = "Beep")]
    public class ReportCategoryNode : IBranch 
    {
        public ReportCategoryNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            // IconImageName = pimagename;

            if (pID != 0)

            {
                ID = pID;
                BranchID = pID;
            }
        }
        public bool Visible { get; set; } = true;
        public string MenuID { get; set; }
        public bool IsDataSourceNode { get; set; } = false;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public IDataSource DataSource { get ; set ; }
        public string DataSourceName { get ; set ; }
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get ; set ; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public EntityStructure EntityStructure { get ; set ; }
        public int MiscID { get ; set ; }
        public string Name { get ; set ; }
        public string BranchText { get; set; } = "Category";
        public int Level { get ; set ; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Category;
        public int BranchID { get ; set ; }
        public string IconImageName { get ; set ; }
        public string BranchStatus { get ; set ; }
        public int ParentBranchID { get ; set ; }
        public string BranchDescription { get ; set ; }
        public string BranchClass { get; set; } = "REPORT";
        public object TreeStrucure { get ; set ; }
        public  IAppManager  Visutil  { get ; set ; }

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }

        // public event EventHandler<PassedArgs> BranchSelected;
        // public event EventHandler<PassedArgs> BranchDragEnter;
        // public event EventHandler<PassedArgs> BranchDragDrop;
        // public event EventHandler<PassedArgs> BranchDragLeave;
        // public event EventHandler<PassedArgs> BranchDragClick;
        // public event EventHandler<PassedArgs> BranchDragDoubleClick;
        // public event EventHandler<PassedArgs> ActionNeeded;

        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {

            try
            {

                foreach (CategoryFolder p in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == "REPORT" && x.FolderName == BranchText))
                {
                    foreach (string item in p.items)
                    {
                   //     AppTemplate i = DMEEditor.ConfigEditor.ReportsDefinition.Where(x => x.Name == item).FirstOrDefault();

                        //if (i != null)
                        //{
                        //    CreateReportNode(i.ID, i.Name); //Path.Combine(i.FilePath, i.FileName)
                        //}
                    }

                }
              
            }
            catch (Exception ex)
            {
                string mes = "Could not Add View to Category";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo MenuItemClicked(string ActionNam)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo RemoveChildNodes()
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {
                    ID = pID;
                }

                //   DMEEditor.AddLogMessage("Success", "Set Config OK", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        #endregion "Interface Methods"
        #region "Exposed Interface"

        #endregion Exposed Interface"
        #region "Other Methods"
        private IBranch CreateReportNode(int id, string ReportName)
        {
            ReportNode viewbr = null;
            try
            {

                viewbr = new ReportNode(TreeEditor, DMEEditor, this, ReportName, TreeEditor.SeqID, EnumPointType.DataPoint, "report.png");
                TreeEditor.Treebranchhandler.AddBranch(this, viewbr);
                viewbr.CreateChildNodes();
                TreeEditor.AddBranchToParentInBranchsOnly(this,viewbr);

                DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                viewbr = null;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };

            return viewbr;
        }
        #endregion"Other Methods"
    }
}
