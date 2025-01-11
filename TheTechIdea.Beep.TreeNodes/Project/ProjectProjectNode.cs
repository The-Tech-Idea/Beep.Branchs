using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.FileManager;
using TheTechIdea.Beep;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Vis;



namespace TheTechIdea.Beep.TreeNodes.Project
{
    [AddinAttribute(Caption = "File", BranchType = EnumPointType.Function, Name = "FileEntityNode.Beep", misc = "Beep", iconimage = "file.png", menu = "Beep", ObjectType = "Beep")]
    public class ProjectProjectNode:IBranch
    {
        public ProjectProjectNode(string url,ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, RootFolder rootFolder)
        {
            Url=url;


            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode.ID;
            BranchText = pBranchText;
            BranchType = EnumPointType.Function;
            DataSourceName = pParentNode.DataSourceName;
           // IconImageName = "folder.png";
            RootFolder = rootFolder;
            if (pID != 0)

            {
                ID = pID;
                BranchID = pID;
            }
        }
        #region "Properties"
        public string MenuID { get; set; }
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
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "project.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "PROJECT";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public IVisManager Visutil { get; set; }
        public int MiscID { get; set; }

        public string Url { get; set; }


        // public event EventHandler<PassedArgs> BranchSelected;
        // public event EventHandler<PassedArgs> BranchDragEnter;
        // public event EventHandler<PassedArgs> BranchDragDrop;
        // public event EventHandler<PassedArgs> BranchDragLeave;
        // public event EventHandler<PassedArgs> BranchDragClick;
        // public event EventHandler<PassedArgs> BranchDragDoubleClick;
        // public event EventHandler<PassedArgs> ActionNeeded;
        #endregion "Properties"
        public bool Visible { get; set; } = true;

        public bool IsDataSourceNode { get; set; } = false;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public RootFolder RootFolder { get; set; }
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            NodesHelpers.CreateProjectStructure( this,RootFolder, TreeEditor, DMEEditor, Visutil);
            return DMEEditor.ErrorObject; ;
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {
            return DMEEditor.ErrorObject; throw new NotImplementedException();
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
        public void CreateProjectStructure(IBranch br,Folder folder, string path)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(path);
            foreach (string file in files)
            {   
               
                string filename = Path.GetFileName(file);
                if (!TreeEditor.Branches.Any(b => b.ParentBranchID == br.ID && b.BranchText.Equals(filename,StringComparison.InvariantCultureIgnoreCase)))
                {
                    ProjectFileNode projectFile = new ProjectFileNode(TreeEditor, DMEEditor, br, filename, TreeEditor.SeqID,folder,null);
                    projectFile.ParentBranchID = br.ID;
                    TreeEditor.treeBranchHandler.AddBranch(br, projectFile);
                    projectFile.CreateChildNodes();
                }
             
            }
          
        }
        [CommandAttribute(Caption = "Remove Project", Hidden = false, DoubleClick = true, iconimage = "remove.png")]
        public IErrorsInfo Removeproject()
        {

            try
            {
                RootFolder f=DMEEditor.ConfigEditor.Projects.FirstOrDefault(p=>p.Url.Equals(Url,StringComparison.InvariantCultureIgnoreCase));
                if(f != null)
                {
                    DMEEditor.ConfigEditor.Projects.Remove(f);
                    DMEEditor.ConfigEditor.SaveProjects();
                    TreeEditor.treeBranchHandler.RemoveBranch(this);
                }
              
                DMEEditor.AddLogMessage("Success", "Removed Project Folder ", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Remove File";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Refresh", Hidden = false, DoubleClick = true, iconimage = "refresh.png")]
        public IErrorsInfo refreshproject()
        {
            try
            {
                if (string.IsNullOrEmpty(RootFolder.Url))
                {
                    NodesHelpers.CreateProjectStructure(this, RootFolder.Url, TreeEditor, DMEEditor, Visutil);
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
        [CommandAttribute(Caption = "Add Folder", Hidden = false, DoubleClick = true, iconimage = "folder.png")]
        public IErrorsInfo addfolder()
        {
            try
            {
                if (!string.IsNullOrEmpty(RootFolder.Url))
                {
                    string foldername = string.Empty;
                    Visutil.Controlmanager.InputBox("Enter Folder Name", "Folder Name", ref foldername);
                    //---- check if project folder exist
                    if (!string.IsNullOrEmpty(foldername))
                    {
                        if(RootFolder.Folders!=null)
                        {
                            if (RootFolder.Folders.Count > 0)
                            {
                                if (RootFolder.Folders.Any(p => p.Name.Equals(foldername, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    DMEEditor.AddLogMessage("Beep", $"Folder already exist {foldername}", DateTime.Now, -1, null, Errors.Failed);
                                    return DMEEditor.ErrorObject; ;
                                }
                            }
                        }
                        else
                        {
                            RootFolder.Folders= new List<Folder>();
                        }
                        Folder f = new Folder();
                        f.Name = foldername;
                        f.Url = RootFolder.Url;
                        RootFolder.Folders.Add(f);
                        int idx = DMEEditor.ConfigEditor.Projects.IndexOf(RootFolder);

                        if (idx > -1)
                        {
                            NodesHelpers.CreateProjectFolder(RootFolder, f,this,TreeEditor,DMEEditor  );
                            DMEEditor.ConfigEditor.Projects[idx] = RootFolder;
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
