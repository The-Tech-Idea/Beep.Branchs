using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea.Beep.FileManager;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;

using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.TreeNodes.Project;
using TheTechIdea.Beep.TreeNodes.Files;

namespace TheTechIdea.Beep.TreeNodes
{
    public static class NodesHelpers
    {
        private static  List<string> files=new List<string>();
        public static List<ConnectionProperties> LoadFiles(IDMEEditor DMEEditor, IAppManager Visutil)
        {
            
            List<ConnectionProperties> retval = new List<ConnectionProperties>();
            try
            {
                string extens = DMEEditor.ConfigEditor.CreateFileExtensionString();
                List<string> filenames = new List<string>();
                filenames = Visutil.DialogManager.LoadFilesDialog("*", DMEEditor.ConfigEditor.Config.Folders.Where(c => c.FolderFilesType == FolderFileTypes.DataFiles).FirstOrDefault().FolderPath, extens);
                retval = DMEEditor.Utilfunction.LoadFiles(filenames.ToArray());
                return retval;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                return null;
            };
        }
        public static List<ConnectionProperties> LoadFiles(string[] filenames,IDMEEditor DMEEditor, IAppManager Visutil)
        {
          
            List<ConnectionProperties> retval = new List<ConnectionProperties>();
            try
            {
//                string extens = DMEEditor.ConfigEditor.CreateFileExtensionString();
                retval = DMEEditor.Utilfunction.LoadFiles(filenames);
                return retval;
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                DMEEditor.AddLogMessage(ex.Message, "Could not Load Files ", DateTime.Now, -1, mes, Errors.Failed);
                return null;
            };
        }
        public static IBranch CreateCategoryNode(CategoryFolder p, IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            FileCategoryNode categoryBranch = null;
            try
            {

                categoryBranch = new FileCategoryNode(TreeEditor, DMEEditor, br, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, "category.png");
                TreeEditor.Treebranchhandler.AddBranch(br, categoryBranch);
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
        public static IBranch CreateFileNode(ConnectionProperties conn, IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            FileEntityNode viewbr = null;
            try
            {
                string ext = Path.GetExtension(conn.ConnectionName).Remove(0, 1);
                string IconImageName = ext + ".png";
                viewbr = new FileEntityNode(TreeEditor, DMEEditor, br, conn.ConnectionName, TreeEditor.SeqID, EnumPointType.DataPoint, IconImageName, conn.GuidID);
                viewbr.DataSource = br.DataSource;
                viewbr.BranchDescription = conn.GuidID.ToString();
                viewbr.GuidID = conn.GuidID.ToString();
                viewbr.DataSourceName = conn.ConnectionName;
                TreeEditor.Treebranchhandler.AddBranch(br, viewbr);
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
        public static IErrorsInfo CreateFileNodes(IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            try
            {
                files = new List<string>();
                foreach (IProject i in DMEEditor.ConfigEditor.Projects.Where(x => x.FolderType == ProjectFolderType.Files))
                {
                    CreateProjectStructure(br, i.GuidID, TreeEditor, DMEEditor, Visutil);
                }
                foreach (ConnectionProperties i in DMEEditor.ConfigEditor.DataConnections.Where(c => c.Category == DatasourceCategory.FILE))
                {
                    string categoryname=TreeEditor.Treebranchhandler.CheckifBranchExistinCategory(i.ConnectionName, "FILE");
                    if ((categoryname == null )|| (br.BranchText == categoryname))
                    {
                        if (!TreeEditor.Branches.Any(p => p.GuidID!=null && p.GuidID.Equals(i.GuidID, StringComparison.InvariantCultureIgnoreCase) && p.BranchClass == br.BranchClass))
                        {
                            if (!files.Contains(i.FilePath))
                            {
                                CreateFileNode(i, br, TreeEditor, DMEEditor, Visutil);
                            }
                            
                        }
                    }
                    
                    
                }
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == "FILE"))
                {
                    if (!TreeEditor.Branches.Any(p => !string.IsNullOrEmpty(p.BranchText) && p.BranchText.Equals(i.FolderName, StringComparison.InvariantCultureIgnoreCase) && p.BranchType == EnumPointType.Category && p.BranchClass == br.BranchClass))
                    {
                        CreateCategoryNode(i, br, TreeEditor, DMEEditor, Visutil);
                    }
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
        public static IBranch CreateFileNode(string FileName, IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            FileEntityNode viewbr = null;
            try
            {
               ;
                
                string ext = Path.GetExtension(FileName).Remove(0, 1);
                string IconImageName = ext + ".png";
                string filename=Path.GetFileName(FileName);
                ConnectionProperties cn = null;
                if (!DMEEditor.ConfigEditor.DataConnectionExist(filename))
                {
                     cn = DMEEditor.Utilfunction.CreateFileDataConnection(filename);
                    DMEEditor.ConfigEditor.AddDataConnection(cn);
                }
                else
                {
                    cn = DMEEditor.ConfigEditor.DataConnections.FirstOrDefault(p => p.ConnectionName.Equals(filename, StringComparison.InvariantCultureIgnoreCase));
                }
                if(cn != null)
                {
                    CreateFileNode(cn, br, TreeEditor, DMEEditor, Visutil);
                }
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
        public static IErrorsInfo AddFile(IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {

            try
            {
                List<ConnectionProperties> files = new List<ConnectionProperties>();
                files = LoadFiles(DMEEditor, Visutil);
                if (files == null)
                {
                    return DMEEditor.ErrorObject;
                }
                foreach (ConnectionProperties f in files)
                {
                    if (!DMEEditor.ConfigEditor.DataConnectionExist(f))
                    {
                        DMEEditor.ConfigEditor.AddDataConnection(f);
                    }
                    IDataSource DataSource = DMEEditor.GetDataSource(f.FileName);
                    if(br.BranchType== EnumPointType.Category)
                    {
                        CategoryFolder x = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == br.BranchText && y.RootName == "FILE").FirstOrDefault();

                        if (x.items.Contains(f.FileName) == false)
                        {
                            x.items.Add(f.FileName);
                        }

                    }

                    CreateFileNode(f, br, TreeEditor, DMEEditor, Visutil);
                }
                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public static IErrorsInfo AddFiles(IBranch br, List<ConnectionProperties> files, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {

            try
            {  
                foreach (ConnectionProperties f in files)
                {
                    if (!DMEEditor.ConfigEditor.DataConnectionExist(f))
                    {
                        DMEEditor.ConfigEditor.AddDataConnection(f);
                    }
                    IDataSource DataSource = DMEEditor.GetDataSource(f.FileName);
                    if (br.BranchType == EnumPointType.Category)
                    {
                        CategoryFolder x = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == br.BranchText && y.RootName == "FILE").FirstOrDefault();

                        if (x.items.Contains(f.FileName) == false)
                        {
                            x.items.Add(f.FileName);
                        }

                    }

                    CreateFileNode(f, br, TreeEditor, DMEEditor, Visutil);
                }
                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public static IErrorsInfo AddFile(IBranch br, ConnectionProperties file, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            try
            {
                bool IsValidDataFile = false;
                if (!DMEEditor.ConfigEditor.DataConnectionExist(file))
                {
                   IsValidDataFile= DMEEditor.ConfigEditor.AddDataConnection(file);
                }
                if(IsValidDataFile)
                {

                    IDataSource DataSource = DMEEditor.GetDataSource(file.FileName);
                    if (br.BranchType == EnumPointType.Category)
                    {
                        CategoryFolder x = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == br.BranchText && y.RootName == "FILE").FirstOrDefault();

                        if (x.items.Contains(file.FileName) == false)
                        {
                            x.items.Add(file.FileName);
                        }

                    }

                    CreateFileNode(file, br, TreeEditor, DMEEditor, Visutil);

                    DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                    DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                    DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);

                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public static IErrorsInfo AddFolder(IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            try
            {
               // List<ConnectionProperties> files = new List<ConnectionProperties>();
                string foldername = Visutil.DialogManager.SelectFolderDialog();
                if (!string.IsNullOrEmpty(foldername))
                {
                    CreateNewProject(br, foldername, TreeEditor, DMEEditor, Visutil, ProjectFolderType.Files);
                    //CreateFilesStructure(br, foldername, TreeEditor, DMEEditor, Visutil);
                }

                DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Database Connection";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public static IErrorsInfo CreateProjectChildNodes(IBranch ProjectsRoot, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                bool missingdir = false;

                CreateProjects(ProjectsRoot, TreeEditor, DMEEditor, Visutil, ProjectFolderType.Project);
               
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Creating Project {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        public static IErrorsInfo RefreshProject(string Projectname, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                IBranch br = TreeEditor.Branches.FirstOrDefault(b => b.Name.Equals(Projectname,StringComparison.InvariantCultureIgnoreCase));
                if(br!=null)
                {
                    RefreshProject(br, TreeEditor, DMEEditor, Visutil);
                }else DMEEditor.AddLogMessage("Beep", $"Could not find Project ", DateTime.Now, 0, null, Errors.Failed);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Creating Project {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        public static IErrorsInfo RefreshProject(IBranch Projectbr, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                bool missingdir = false;
                TreeEditor.Treebranchhandler.RemoveChildBranchs(Projectbr);
                foreach (var item in DMEEditor.ConfigEditor.Projects)
                {
                    string dirname = new System.IO.DirectoryInfo(item.Url).Name;
                    if (Directory.Exists(item.Url))
                    {
                        ProjectProjectNode FolderNode = new ProjectProjectNode(item.Url, TreeEditor, DMEEditor, Projectbr, dirname, TreeEditor.SeqID,item);
                        TreeEditor.Treebranchhandler.AddBranch(Projectbr, FolderNode);
                        CreateProjectStructure(FolderNode, item.Url, TreeEditor, DMEEditor, Visutil);
                    }
                    else
                        missingdir = true;

                }
                if (missingdir)
                {
                    if (Visutil.DialogManager.InputBoxYesNo("Beep", "There are missing Project Directories") == BeepDialogResult.OK)
                    {
                        foreach (var item in DMEEditor.ConfigEditor.Projects)
                        {
                            string dirname = new System.IO.DirectoryInfo(item.Url).Name;
                            if (!Directory.Exists(item.Url))
                            {
                                DMEEditor.ConfigEditor.Projects.Remove(item);
                            }
                        }
                        DMEEditor.ConfigEditor.SaveProjects();
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Creating Project {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        public static void CreateNewProject(IBranch br, string path, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil,ProjectFolderType foldertype)
        {
            //FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            Tuple<IErrorsInfo,FileManager.RootFolder> folder = DMEEditor.Utilfunction.CreateProject(path);
            
            if (folder.Item1.Flag== Errors.Ok)
            {
                RootFolder rootFolder = folder.Item2;
                rootFolder.FolderType = foldertype;

                foreach (FileManager.Folder fld in folder.Item2.Folders)
                {
                    ProjectFolderNode fnode = new ProjectFolderNode(TreeEditor, DMEEditor, br, fld.Name, TreeEditor.SeqID,fld, rootFolder);
                    TreeEditor.Treebranchhandler.AddBranch(br, fnode);
                    TTraverseProjectFolder(rootFolder,fld, fnode, TreeEditor, DMEEditor, Visutil);
                }
            }
            if(foldertype== ProjectFolderType.Project)
            {
                if (folder != null)
                {
                    if (folder.Item2 != null)
                    {
                        RootFolder rootFolder = folder.Item2;
                        rootFolder.FolderType = foldertype;
                        if (DMEEditor.ConfigEditor.Projects == null)
                        {
                            DMEEditor.ConfigEditor.Projects = new List<RootFolder>();
                        }
                        if (DMEEditor.ConfigEditor.Projects.Any(p => p.Url == rootFolder.Url))
                        {
                            int idx = DMEEditor.ConfigEditor.Projects.FindIndex(p => p.Url == rootFolder.Url);
                            DMEEditor.ConfigEditor.Projects[idx] = rootFolder;
                        }
                        else
                        {
                            DMEEditor.ConfigEditor.Projects.Add(rootFolder);
                        }
                        DMEEditor.ConfigEditor.SaveProjects();
                    }
                }
            }
           
           
        }
        #region "Create Project Nodes"
        public static void CreateProjectStructure(IBranch br, string projectGuidID, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            IBranch projectFolderNode;
           // FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            RootFolder folder = (RootFolder)DMEEditor.ConfigEditor.Projects.FirstOrDefault(p => p.GuidID == projectGuidID);
            if (folder != null)
            {
                if (folder.Folders != null)
                {
                    foreach (FileManager.Folder fld in folder.Folders)
                    {
                        TTraverseProjectFolder(folder,fld, br, TreeEditor, DMEEditor, Visutil);
                    }

                }
             
            }


        }
        public static void CreateProjectStructure(IBranch br, RootFolder folder, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            IBranch projectFolderNode;
            // FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            
            if (folder != null)
            {
                if (folder.Folders != null)
                {
                    foreach (FileManager.Folder fld in folder.Folders)
                    {
                        TTraverseProjectFolder(folder,fld, br, TreeEditor, DMEEditor, Visutil);
                    }

                }

            }


        }
        public static void CreateProjects(IBranch br, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil, ProjectFolderType foldertype)
        {
            //FileHelpers fileHelpers = new FileHelpers(DMEEditor);
            List<RootFolder> projects = DMEEditor.ConfigEditor.Projects.Where(p => p.FolderType == foldertype).ToList();
            if (projects != null)
            {
                if (projects.Count>0)
                {
                    foreach (RootFolder prj in projects)
                    {
                        ProjectProjectNode fnode = new ProjectProjectNode(prj.Url,TreeEditor, DMEEditor, br, prj.Name, TreeEditor.SeqID,prj);
                        TreeEditor.Treebranchhandler.AddBranch(br, fnode);
                        fnode.CreateChildNodes();
                        
                    }
                  
                }
               
            }
        }
        public static void TTraverseProjectFolder(RootFolder root,Folder fld, IBranch FolderNode, ITree TreeEditor, IDMEEditor DMEEditor, IAppManager Visutil)
        {
            foreach (FFile fFile in fld.Files)
            {
                string IconImageName = fFile.Name + ".png";
                
                if (!TreeEditor.Branches.Any(b => b.ParentBranchID == FolderNode.ID && b.BranchText.Equals(fFile.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    files.Add(Path.Combine(fFile.Url, fFile.Name));
                    if (IsFileValid(fFile.Name, DMEEditor))
                    {
                        CreateFileNode(Path.Combine(fFile.Url, fFile.Name), FolderNode, TreeEditor, DMEEditor, Visutil);
                    }
                    else
                    {
                        ProjectFileNode projectFileNode = new ProjectFileNode(TreeEditor, DMEEditor, FolderNode, fFile.Name, TreeEditor.SeqID, fld);
                        projectFileNode.FileDir = Path.GetDirectoryName(fFile.Url);
                        TreeEditor.Treebranchhandler.AddBranch(FolderNode, projectFileNode);
                    }
                }
                
              
                
            }
            if (fld.Folders.Count > 0)
            {
                foreach (var childfolder in fld.Folders)
                {
                    string dirname = childfolder.Name;
                    if (!TreeEditor.Branches.Any(b => b.ParentBranchID == FolderNode.ID && b.BranchText.Equals(dirname, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ProjectFolderNode folder = new ProjectFolderNode(TreeEditor, DMEEditor, FolderNode, childfolder.Name, TreeEditor.SeqID,  childfolder, root);
                        TreeEditor.Treebranchhandler.AddBranch(FolderNode, folder);
                        folder.CreateChildNodes();
                    }
                }
            }

        }
        public static Folder CreateProjectFolder(RootFolder root,string newfoldername,string path,string dirname, IBranch parentFolderNode, ITree TreeEditor, IDMEEditor DMEEditor)
        {
            Folder f = new Folder();
            f.Name = newfoldername;
            f.Url = path;
            if (!TreeEditor.Branches.Any(b => b.ParentBranchID == parentFolderNode.ID && b.BranchText.Equals(dirname, StringComparison.InvariantCultureIgnoreCase)))
            {
 
                ProjectFolderNode folder = new ProjectFolderNode(TreeEditor, DMEEditor, parentFolderNode, newfoldername, TreeEditor.SeqID, f,root);
                TreeEditor.Treebranchhandler.AddBranch(parentFolderNode, folder);
                folder.CreateChildNodes();
            }else
                f = null;
            return f;
            
        }
        public static void CreateProjectFolder(RootFolder root, Folder newfolder, IBranch parentFolderNode, ITree TreeEditor, IDMEEditor DMEEditor)
        {
           
            if (!TreeEditor.Branches.Any(b => b.ParentBranchID == parentFolderNode.ID && b.BranchText.Equals(newfolder.Name, StringComparison.InvariantCultureIgnoreCase)))
            {

                ProjectFolderNode folder = new ProjectFolderNode(TreeEditor, DMEEditor, parentFolderNode, newfolder.Name, TreeEditor.SeqID, newfolder,root);
                TreeEditor.Treebranchhandler.AddBranch(parentFolderNode, folder);
                folder.CreateChildNodes();
            }
           
          

        }
        #endregion "Create Project Nodes"
        #region "Create Files Nodes"
        public static bool IsFileValid(string filename, IDMEEditor DMEEditor)
        {
            bool retval = false;
            string ext = Path.GetExtension(filename).Replace(".", "").ToLower();
            List<ConnectionDriversConfig> clss = DMEEditor.ConfigEditor.DataDriversClasses.Where(p => p.extensionstoHandle != null).ToList();
            if (clss != null)
            {
                IEnumerable<string> extensionslist = clss.Select(p => p.extensionstoHandle);
                string extstring = string.Join(",", extensionslist);
                List<string> exts = extstring.Split(',').Distinct().ToList();
                retval = exts.Contains(ext);
            }
            return retval;
        }
        public static string CheckifBranchExistinCategory(string BranchName, string pRootName, IDMEEditor DMEEditor)
        {
            //bool retval = false;
            List<CategoryFolder> ls = DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == pRootName).ToList();
            foreach (CategoryFolder item in ls)
            {
                foreach (string f in item.items)
                {
                    if (f == BranchName)
                    {
                        return item.FolderName;
                    }
                }
            }
            return null;
        }
        #endregion "Create Files Nodes"


    }
}
