using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis;


using TheTechIdea;
using TheTechIdea.Beep.AppManager;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.TreeNodes.Reports
{
    [AddinAttribute(Caption = "Reports", BranchType = EnumPointType.Root, Name = "ReportRootNode.Beep", misc = "Beep", iconimage = "report.png", menu = "Beep",ObjectType ="Beep")]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "DATASOURCEROOT")]
    public class QueryRootNode  : IBranch , IOrder
    {
        public QueryRootNode()
        {

        }
        public QueryRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string ConnectionName)
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
        #region "Properties"
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public int Order { get; set; } = 10;
        public string Name { get; set; }
        public string BranchText { get; set; } = "Reports";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "reports.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "REPORT";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IVisManager  Visutil { get; set; }
        public int MiscID { get; set; }


        // public event EventHandler<PassedArgs> BranchSelected;
        // public event EventHandler<PassedArgs> BranchDragEnter;
        // public event EventHandler<PassedArgs> BranchDragDrop;
        // public event EventHandler<PassedArgs> BranchDragLeave;
        // public event EventHandler<PassedArgs> BranchDragClick;
        // public event EventHandler<PassedArgs> BranchDragDoubleClick;
        // public event EventHandler<PassedArgs> ActionNeeded;
        #endregion "Properties"
        #region "Interface Methods"
        public IBranch CreateCategoryNode( CategoryFolder p)
        {
            ReportCategoryNode Category = null;
            try
            {
                Category = new ReportCategoryNode(TreeEditor, DMEEditor, this, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, TreeEditor.CategoryIcon);
                TreeEditor.Treebranchhandler.AddBranch(this, Category);
                this.TreeEditor.AddBranchToParentInBranchsOnly(this,Category);
                Category.CreateChildNodes();

            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error Creating Category Node ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return Category;

        }
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                CreateNodes();

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
        [CommandAttribute(Caption = "Create Report")]
        public IErrorsInfo CreateReport()
        {

            try
            {
                string[] args = { "New Query Entity", null, null };
                List<ObjectItem> ob = new List<ObjectItem>(); ;
                ObjectItem it = new ObjectItem();
                it.obj = this;
                it.Name = "Branch";
                ob.Add(it);
                IBranch RootBranch = TreeEditor.Branches[TreeEditor.Branches.FindIndex(x => x.BranchClass == "REPORT" && x.BranchType == EnumPointType.Root)];
                it = new ObjectItem();
                it.obj = RootBranch;
                it.Name = "RootReportBranch";
                ob.Add(it);
                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = null,
                    CurrentEntity = null,

                    ObjectType = "NEWREPORT",
                    DataSource = null,
                    ObjectName = null,

                    Objects = ob,

                    DatasourceName = null,
                    EventType = "NEWREPORT"

                };
               // ActionNeeded?.Invoke(this, Passedarguments);
                Visutil.ShowPage("uc_reportdefinition", Passedarguments,DisplayType.InControl);

               // DMEEditor.AddLogMessage("Success", "Created Query Entity", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Open Report Editor";
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
                //foreach (var i in DMEEditor.ConfigEditor.ReportWritersClasses)
                //{

                //    if (TreeEditor.Treebranchhandler.CheckifBranchExistinCategory(i.Name, "REPORT") == null)
                //    {
                //        // ObjectDataSourcetemp = i.FileName;

                //        CreateReportNode(i.ID, i.Name);


                //    }
                //}
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.RootName == "APP"))
                {

                    CreateCategoryNode(i);


                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Add Report Nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;

        }
       
        private IBranch CreateReportNode(int id, string ReportName)
        {
            ReportNode viewbr = null;
            try
            {

                viewbr = new ReportNode(TreeEditor, DMEEditor, this, ReportName, TreeEditor.SeqID, EnumPointType.Function, "report.png");
                TreeEditor.Treebranchhandler.AddBranch(this, viewbr);
                viewbr.CreateChildNodes();
                TreeEditor.AddBranchToParentInBranchsOnly(this,viewbr);

                DMEEditor.AddLogMessage("Success", "Added Report", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Report";
                viewbr = null;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };

            return viewbr;
        }

       
        #endregion"Other Methods"
    }
}
