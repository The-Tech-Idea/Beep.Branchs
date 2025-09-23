using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheTechIdea.Beep;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.FileManager;



namespace TheTechIdea.Beep.TreeNodes.Project
{
    [AddinAttribute(Caption = "Folder", BranchType = EnumPointType.Category, Name = "ProjectFolderNode.Beep", misc = "Beep", iconimage = "folder.png", menu = "Beep", ObjectType = "Beep")]
    public class ProjectFolderNode : IBranch
    {
        public ProjectFolderNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, Folder folder,RootFolder rootFolder)
        {



            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = EnumPointType.Category;
            DataSourceName = pParentNode.DataSourceName;
            IconImageName = "folder.png";
            ParentBranch= pParentNode;
            _rootFolder= rootFolder;
            _folder= folder;
            if (pID != 0)

            {
                ID = pID;
                BranchID = pID;
            }
        }
        #region "Properties"
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "Files";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Category;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "file.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "PROJECT";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int MiscID { get; set; }


        // public event EventHandler<PassedArgs> BranchSelected;
        // public event EventHandler<PassedArgs> BranchDragEnter;
        // public event EventHandler<PassedArgs> BranchDragDrop;
        // public event EventHandler<PassedArgs> BranchDragLeave;
        // public event EventHandler<PassedArgs> BranchDragClick;
        // public event EventHandler<PassedArgs> BranchDragDoubleClick;
        // public event EventHandler<PassedArgs> ActionNeeded;
        #endregion "Properties"
        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;
        
        public bool IsDataSourceNode { get; set; } = false;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public RootFolder _rootFolder { get; set; }
        public Folder _folder { get; set; }
      
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            NodesHelpers.TTraverseProjectFolder(_rootFolder, _folder,this, TreeEditor, DMEEditor, Visutil);
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo MenuItemClicked(string ActionNam)
        {
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo RemoveChildNodes()
        {
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;

            }
            catch (Exception ex)
            {
                string mes = "Could not Set Config";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Add Folder", Hidden = false, DoubleClick = true, iconimage = "folder.png")]
        public IErrorsInfo addfolderinafolder()
        {
            try
            {
                DialogReturn res= new DialogReturn();
                if (!string.IsNullOrEmpty(_folder.Url))
                {
                    string foldername = string.Empty;
                   res= Visutil.DialogManager.InputBox("Enter Folder Name", "Folder Name");
                    foldername = res.Value;
                    //---- check if project folder exist
                    if (!string.IsNullOrEmpty(foldername))
                    {
                        if (_folder.Folders != null)
                        {
                            if (_folder.Folders.Count > 0)
                            {
                                if (_folder.Folders.Any(p => p.Name.Equals(foldername, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    DMEEditor.AddLogMessage("Beep", $"Folder already exist {foldername}", DateTime.Now, -1, null, Errors.Failed);
                                    return DMEEditor.ErrorObject; ;
                                }
                            }
                        }
                        else
                        {
                            _folder.Folders = new List<Folder>();
                        }

                        Folder f = new Folder();
                        f.Name = foldername;
                        f.Url = _folder.Url;
                        _folder.Folders.Add(f);
                        int rootidx = DMEEditor.ConfigEditor.Projects.IndexOf(_rootFolder);
                      //  int idx = DMEEditor.ConfigEditor.Projects[rootidx].Folders.IndexOf(_folder);

                        if (rootidx > -1)
                        {
                            NodesHelpers.CreateProjectFolder(_rootFolder, f, this, TreeEditor, DMEEditor);
                            DMEEditor.ConfigEditor.Projects[rootidx].Folders.Add(_folder);
                            CreateChildNodes();
                        }

                    }
                }



                DMEEditor.ConfigEditor.SaveProjects();
                DMEEditor.AddLogMessage("Success", "Refreshed Project Folder ", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Remove File";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
    }
}
