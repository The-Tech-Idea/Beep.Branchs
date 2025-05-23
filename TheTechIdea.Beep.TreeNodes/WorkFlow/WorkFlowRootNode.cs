﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Workflow;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Addin;




namespace TheTechIdea.Beep.TreeNodes.WorkFlow
{
    [AddinAttribute(Caption = "WorkFlows", BranchType = EnumPointType.Root, Name = "WorkFlowRootNode.Beep", misc = "Beep", iconimage = "workflow.png", menu = "DataManagement", ObjectType ="Beep")]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "DATAMANAGMENET", RootNodeName = "DataManagementGenereNode")]
    public class WorkFlowRootNode  : IBranch ,IOrder
    {
        public WorkFlowRootNode()
        {

        }  
        public WorkFlowRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string ConnectionName)
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
        #region "Properties"
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public int Order { get; set; } = 8;
        public string Name { get; set; }
        public string BranchText { get; set; } = "WorkFlows";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "workflow.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "WORKFLOW";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IAppManager  Visutil { get; set; }
        public int MiscID { get; set; }
       // public AddinTreeStructure AddinTreeStructure { get; set; }

       // public event EventHandler<PassedArgs> BranchSelected;
       // public event EventHandler<PassedArgs> BranchDragEnter;
       // public event EventHandler<PassedArgs> BranchDragDrop;
       // public event EventHandler<PassedArgs> BranchDragLeave;
       // public event EventHandler<PassedArgs> BranchDragClick;
       // public event EventHandler<PassedArgs> BranchDragDoubleClick;
       // public event EventHandler<PassedArgs> ActionNeeded;
        #endregion "Properties"
        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                CreateNodes();
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == "WORKFLOW"))
                {

                    CreateCategoryNode(i);


                }
              //  CreateNodes();

                DMEEditor.AddLogMessage("Success", "Added Child Nodes", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Child Nodes";
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
                //ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
                //BranchText = pBranchText;
                //BranchType = pBranchType;
                //IconImageName = pimagename;
                //if (pID != 0)
                //{
                //    ID = pID;
                //}

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
       
        [CommandAttribute(Caption = "New", Hidden = false)]
        public IErrorsInfo NewWK()
        {

            try
            {
                string[] args = { "New Query Entity", null, null };
                List<ObjectItem> ob = new List<ObjectItem>(); ;
                ObjectItem it = new ObjectItem();
                it.obj = this;
                it.Name = "Branch";
                ob.Add(it);
                
                PassedArgs Passedarguments = new PassedArgs
                {  // Obj= obj,
                    Addin = null,
                    AddinName = null,
                    AddinType = null,
                    DMView = null,
                    CurrentEntity = null,
                    ObjectName = null,
                    Id = BranchID,
                    ObjectType = "WORKFLOW",
                    DataSource = DataSource,
                    EventType = "NEW",
                     Objects=ob

                };


                Visutil.ShowPage("uc_WorkFlowManagerMainScreen",  Passedarguments,DisplayType.InControl  );




                DMEEditor.AddLogMessage("Success", "Shown Module " + BranchText, DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Config Module " + BranchText;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
       
        [CommandAttribute(Caption = "DoubleClick", Hidden = true, DoubleClick = true)]
        public IErrorsInfo DoubleClick()
        {

            try
            {
                string[] args = { BranchText };
                PassedArgs Passedarguments = new PassedArgs
                {  // Obj= obj,
                    Addin = null,
                    AddinName = null,
                    AddinType = null,
                    DMView = null,
                    CurrentEntity = null,
                    ObjectName = null,
                    Id = BranchID,
                    ObjectType = "UserControl",
                    DataSource = DataSource,
                    EventType = "Run"

                };


                Visutil.ShowPage("uc_WorkFlowManagerMainScreen", Passedarguments);

                DMEEditor.AddLogMessage("Success", "Shown Module " + BranchText, DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Config Module " + BranchText;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        #endregion Exposed Interface"
        #region "Other Methods"
        public IErrorsInfo CreateNodes()
        {

            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                foreach (IWorkFlow item in DMEEditor.ConfigEditor.WorkFlows)
                {
                    WorkFlowEntityNode  en = new WorkFlowEntityNode(TreeEditor, DMEEditor, this,item.DataWorkFlowName, TreeEditor.SeqID, EnumPointType.DataPoint, "workflowentity.png");
                    en.DataSource = DataSource;
                    TreeEditor.Treebranchhandler.AddBranch(this, en);
                    TreeEditor.AddBranchToParentInBranchsOnly(this,en);
                }


                DMEEditor.AddLogMessage("Success", "Created child Nodes", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Create child Nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }
        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            WorkFlowCategoryNode categoryBranch = null;
            try
            {
                 categoryBranch = new WorkFlowCategoryNode(TreeEditor, DMEEditor, this, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, "category.png");
                TreeEditor.Treebranchhandler.AddBranch(this, categoryBranch);
                TreeEditor.AddBranchToParentInBranchsOnly(this,categoryBranch);
                categoryBranch.CreateChildNodes();


            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error Creating Category  View Node ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return categoryBranch;

        }

       
        #endregion"Other Methods"
    }
}
