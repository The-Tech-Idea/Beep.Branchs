using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    [AddinAttribute(Caption = "DataView", BranchType = EnumPointType.Category, Name = "DataViewCategoryNode.Beep", misc = "Beep", iconimage = "dataview.png", menu = "Beep", ObjectType = "Beep")]
    public class DataViewCategoryNode  : IBranch 
    {
        public DataViewCategoryNode()
        {

        }
        public DataViewCategoryNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
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
                BranchID = ID;
            }
        }
        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;
        #region "Properties"
        public bool IsDataSourceNode { get; set; } = true;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } 
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Category;
        public int BranchID { get; set; }
        public string IconImageName { get; set; }
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VIEW";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IVisManager  Visutil { get; set; }
        public int MiscID { get; set; }

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
        #endregion "Properties"
        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {
            try
            {

                foreach (CategoryFolder p in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == "VIEW" && x.FolderName == BranchText))
                {
                    foreach (string item in p.items)
                    {
                        ConnectionProperties i = DMEEditor.ConfigEditor.DataConnections.Where(x => x.ConnectionName == item).FirstOrDefault();

                        if (i != null)
                        {
                            CreateViewNode(i); //Path.Combine(i.FilePath, i.FileName)
                        }
                     

                      //  i.Drawn = true;
                    }



                }
                //DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
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

                DMEEditor.AddLogMessage("Success", "Set Config OK", DateTime.Now, 0, null, Errors.Ok);
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
        private IBranch CreateViewNode(ConnectionProperties i)
        {
            DataViewNode viewbr = null;
            try
            {

                viewbr = new DataViewNode(TreeEditor, DMEEditor, this, i.ConnectionName, TreeEditor.SeqID, EnumPointType.DataPoint, "dataview.png", i.ConnectionName);

                viewbr.DataSource = DataSource;
                viewbr.DataSourceName = i.ConnectionName;
                viewbr.GuidID = i.GuidID;
                TreeEditor.Treebranchhandler.AddBranch(this, viewbr);
               

                DMEEditor.AddLogMessage("Success", "Added DataView Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add DataView Connection";
                viewbr = null;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };

            return viewbr;
        }
        #endregion"Other Methods"
    }
}
