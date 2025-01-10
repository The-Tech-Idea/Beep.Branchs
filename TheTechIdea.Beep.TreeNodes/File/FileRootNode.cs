using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheTechIdea.Beep.Addin;
using TheTechIdea;
using TheTechIdea.Beep;
using  Beep.Vis.Module;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Project;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Util;

namespace  BeepEnterprize.Vis.Module
{
    [AddinAttribute(Caption = "Files", BranchType = EnumPointType.Root, Name = "FileRootNode.Beep", misc = "Beep", iconimage = "file.png", menu = "DataSource", ObjectType ="Beep",Category = DatasourceCategory.FILE)]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "DATASOURCEROOT")]
    public class FileRootNode : IBranch ,IOrder 
    {
        public FileRootNode()
        {
            IsDataSourceNode = true;

        }
        public FileRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string ConnectionName)
        {



            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            //ParentBranchID = pParentNode.ID;
            //BranchText = pBranchText;
            //BranchType = pBranchType;
            //IconImageName = pimagename;

            //if (pID != 0)

            //{
            //    ID = pID;
            //    BranchID = pID;
            //}
        }
        public bool Visible { get; set; } = true;

        public bool IsDataSourceNode { get; set; } = true;
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
        public int Order { get; set; } = 4;
        public string Name { get; set; }
        public string BranchText { get; set; } = "Files";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "folders.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "FILE";
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
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            FileCategoryNode categoryBranch = null;
            try
            {
                IBranch parent = TreeEditor.Branches.FirstOrDefault(x => x.ID == p.ID);
                if (parent == null)
                {
                    parent = this;
                }
                categoryBranch = new FileCategoryNode(TreeEditor, DMEEditor, parent, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, "category.png");
                TreeEditor.treeBranchHandler.AddBranch(parent, categoryBranch);
                ChildBranchs.Add(categoryBranch);
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
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                NodesHelpers.CreateFileNodes(this, TreeEditor, DMEEditor, Visutil);

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
                //ParentBranchID = pParentNode.ID;
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
     
        [CommandAttribute(Caption = "Add File", Hidden = false, iconimage = "add.png", ObjectType = "Beep")]
        public IErrorsInfo AddFile()
        {

            try
            {
              
                NodesHelpers.AddFile(this, TreeEditor, DMEEditor, Visutil);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Add Folder", Hidden = false, iconimage = "folder.png", ObjectType = "Beep")]
        public IErrorsInfo AddFolder()
        {

            try
            {
              
                NodesHelpers.AddFolder(this,TreeEditor,DMEEditor, Visutil);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        #endregion Exposed Interface"
        #region "Other Methods"
     
      
        #endregion"Other Methods"
    }
}
