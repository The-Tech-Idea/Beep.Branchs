using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.FileManager;

using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.TreeNodes.Project
{
    [AddinAttribute(Caption = "Projects", BranchType = EnumPointType.Root, Name = "ProjectRootNode.Beep", misc = "Beep", iconimage = "projectsmanagement.png", menu = "Beep", ObjectType = "Beep")]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "PROJECTROOT")]
    public class ProjectRootBranch : IBranch
    {
        public ProjectRootBranch()
        {

        }
        #region "Properties"
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "Projects";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "projectsmanagement.png";
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
        public bool Visible { get; set; } = true;
        public string MenuID { get; set; }
        public bool IsDataSourceNode { get; set; } = false;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
               // TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                NodesHelpers.CreateProjectChildNodes(this, TreeEditor, DMEEditor, Visutil);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Creating Project {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
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
            TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
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
     
        [CommandAttribute(Caption = "Add Project", Hidden = false, DoubleClick = true, iconimage = "edit.png")]
        public IErrorsInfo addproject()
        {

            try
            {

                string folderpath = Visutil.Controlmanager.SelectFolderDialog();
                if (!string.IsNullOrEmpty(folderpath ))
                {
                    string foldername = string.Empty;
                     Visutil.Controlmanager.InputBox("Enter Project Name", "Project Name", ref foldername);
                    //---- check if project folder exist
                    if (!string.IsNullOrEmpty(foldername))
                    {
                        string fullprojectpath=Path.Combine(folderpath, foldername);
                        if (DMEEditor.ConfigEditor.Projects.Any(p => p.Url.Equals(fullprojectpath, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            DMEEditor.AddLogMessage("Beep", $"Folder already exist {fullprojectpath}", DateTime.Now, -1, null, Errors.Failed);
                            return DMEEditor.ErrorObject; ;
                        }
                        if (!Directory.Exists(fullprojectpath))
                        {
                            try
                            {
                                Directory.CreateDirectory(fullprojectpath);
                                RootFolder f = new RootFolder();
                                string dirname = foldername;
                                f.FolderType = ProjectFolderType.Project;
                                f.Url = folderpath;
                                f.Name = dirname;

                                DMEEditor.ConfigEditor.Projects.Add(f);
                                DMEEditor.ConfigEditor.SaveProjects();

                                ProjectProjectNode FolderNode = new ProjectProjectNode(f.Url, TreeEditor, DMEEditor, this, dirname, TreeEditor.SeqID,f);
                                TreeEditor.Treebranchhandler.AddBranch(this, FolderNode);
                                FolderNode.RootFolder= f;
                                FolderNode.CreateChildNodes();
                                //NodesHelpers.CreateProjectStructure(FolderNode, f.GuidID, TreeEditor, DMEEditor, Visutil);
                            }
                            catch (Exception ex)
                            {
                                DMEEditor.AddLogMessage("Beep", $"Error Creating Folder {ex.Message} - {folderpath}", DateTime.Now, -1, null, Errors.Failed);
                                return DMEEditor.ErrorObject; ;
                                
                            }
                           

                        }
                      
                    }
               
                }
                 DMEEditor.AddLogMessage("Success", "Added Project Folder ", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Show File";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
       
    }
}
