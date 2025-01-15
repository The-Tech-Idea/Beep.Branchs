using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.DataView;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    [AddinAttribute(Caption = "DataView", BranchType = EnumPointType.Root, Name = "DataViewRootNode.Beep", misc = "Beep", iconimage = "dataview.png", menu = "DataSource", ObjectType ="Beep",Category = DatasourceCategory.VIEWS)]
    [AddinVisSchema(BranchType = EnumPointType.Root, BranchClass = "DATASOURCEROOT", RootNodeName = "DataSourcesRootNode")]
    public class DataViewRootNode  : IBranch ,IOrder 
    {
        public DataViewRootNode()
        {

        }
        public DataViewRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode!=null? pParentNode.ID : -1;
            //BranchText = pBranchText;
            //BranchType = pBranchType;
            //IconImageName = pimagename;
            ID=pID;
            if (pID != 0)
            {
                ID = pID;
                BranchID = ID;
            }
            //.GetImageIndex(ParentTree, MainNode, "dataview.png");
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
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; }
        public int Order { get; set; } = 0;
        public string Name { get; set; }
        public string BranchText { get; set; } = "DataView";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; }= EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "dataview.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "VIEW";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; }
        public object TreeStrucure { get; set; }
        public  IAppManager  Visutil { get; set; }
        public int MiscID { get; set; }

       // public event EventHandler<PassedArgs> BranchSelected;
       // public event EventHandler<PassedArgs> BranchDragEnter;
       // public event EventHandler<PassedArgs> BranchDragDrop;
       // public event EventHandler<PassedArgs> BranchDragLeave;
       // public event EventHandler<PassedArgs> BranchDragClick;
       // public event EventHandler<PassedArgs> BranchDragDoubleClick;
       // public event EventHandler<PassedArgs> ActionNeeded;

        public IDMDataView DataView { get; set; }
        public int DataViewID { get; set; }

        #endregion "Properties"
        #region "Interface Methods"
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                foreach (ConnectionProperties i in DMEEditor.ConfigEditor.DataConnections.Where(c => c.Category == DatasourceCategory.VIEWS))
                {

                    if (TreeEditor.Treebranchhandler.CheckifBranchExistinCategory(i.ConnectionName, "VIEW") == null)
                    {
                        // ObjectDataSourcetemp = i.FileName;
                        if (!ChildBranchs.Any(p => p.GuidID.Equals(i.GuidID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            CreateViewNode(i);

                            i.Drawn = true;
                        }
                        
                    }
                }
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == "VIEW"))
                {

                    CreateCategoryNode(i);


                }

                //  DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Views";
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

            //    DMEEditor.AddLogMessage("Success", "Set Config OK", DateTime.Now, 0, null, Errors.Ok);
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
      
        [CommandAttribute(Caption = "Create View", ObjectType = "Beep" ,iconimage ="createview.png")]
        public IErrorsInfo CreateView()
        {

            try
            {
                string viewname = null;
                string fullname = null;
                if (Visutil.Controlmanager.InputBox("Create View", "Please Enter Name of View (Name Should not exist already in Views)", ref viewname) == DialogResult.OK)
                {
                    if ((viewname != null) && DMEEditor.ConfigEditor.DataConnectionExist(viewname+".json")==false )
                    {
                       
                        
                        fullname = Path.Combine(DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataView).FirstOrDefault().FolderPath, viewname + ".json");
                        ConnectionProperties f = new ConnectionProperties
                        {

                            FileName = Path.GetFileName(fullname),
                            FilePath = Path.GetDirectoryName(fullname),
                            Ext = Path.GetExtension(fullname),
                            ConnectionName = Path.GetFileName(fullname)
                        };

                        f.Category = DatasourceCategory.VIEWS;
                        f.DriverVersion = "1";
                        f.DriverName = "DataViewReader";

                        DMEEditor.ConfigEditor.DataConnections.Add(f);
                        DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                       
                        IDataViewDataSource ds = (IDataViewDataSource)DMEEditor.GetDataSource(f.ConnectionName);
                        ds.DataView = ds.GenerateView(f.ConnectionName, f.ConnectionName);
                        DataView = ds.DataView;
                        ds.WriteDataViewFile(fullname);
                        IBranch br = CreateViewNode(f);
                        DMEEditor.AddLogMessage("Success", "Added View", DateTime.Now, 0, null, Errors.Ok);

                    }
                    else
                    {
                        //MessageBox.Show("Please Select Other Name, Data Connection by this name Exist");
                    }
                  
                }
                else
                {
                    Visutil.Controlmanager.MsgBox("DM Engine", "Please Try another name . DataSource Exist");
                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Added View ";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Add View File", ObjectType = "Beep", iconimage = "loadviewfile.png")]
        public IErrorsInfo AddViewFile()
        {
            try
            {
                string viewname = null;
                string fullname = null;
                string filename = Visutil.Controlmanager.LoadFileDialog("json", DMEEditor.ConfigEditor.Config.Folders.Where(i => i.FolderFilesType == FolderFileTypes.DataView).FirstOrDefault().FolderPath, "json files (*.json)|*.txt|All files (*.*)|*.*");
                if (!string.IsNullOrEmpty(filename))
                {
                    viewname = Path.GetFileName(filename);
                    if (!string.IsNullOrEmpty(viewname))
                    {
                        if (DMEEditor.ConfigEditor.DataConnectionExist(viewname + ".json") == false)
                        {
                            fullname = filename;  //Path.Combine(Path.GetDirectoryName(openFileDialog1.FileName), Path.GetFileName(openFileDialog1.FileName));
                            ConnectionProperties f = new ConnectionProperties
                            {

                                FileName = Path.GetFileName(fullname),
                                FilePath = Path.GetDirectoryName(fullname),
                                Ext = Path.GetExtension(fullname),
                                ConnectionName = Path.GetFileName(fullname)
                            };

                            f.Category = DatasourceCategory.VIEWS;
                            f.DriverVersion = "1";
                            f.DriverName = "DataViewReader";

                            DMEEditor.ConfigEditor.DataConnections.Add(f);
                            DMEEditor.ConfigEditor.SaveDataconnectionsValues();

                            IDataViewDataSource ds = (IDataViewDataSource)DMEEditor.GetDataSource(f.ConnectionName);
                            DataSource = DMEEditor.GetDataSource(f.ConnectionName);
                            DataView = ds.DataView;
                            ds.WriteDataViewFile(fullname);
                            CreateViewNode(f);
                        }
                        DMEEditor.AddLogMessage("Success", "Added View", DateTime.Now, 0, null, Errors.Ok);
                    }
                 
                }
                else
                {
                    Visutil.Controlmanager.MsgBox("DM Engine", "Please Try another name . DataSource Exist");
                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Added View ";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Create View using Table",Hidden =true, iconimage = "createviewusingtable.png", ObjectType = "Beep")]
        public IErrorsInfo CreateView(IBranch EntitySource)
        {

            try
            {
                string viewname = null;
                string fullname = null;
                if (Visutil.Controlmanager.InputBox("Create View", "Please Enter Name of View (Name Should not exist already in Views)", ref viewname) == DialogResult.OK)
                {
                    if ((viewname != null) && DMEEditor.ConfigEditor.DataConnectionExist(viewname+".json") == false)
                    {


                        fullname = Path.Combine(DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataView).FirstOrDefault().FolderPath, viewname + ".json");
                        ConnectionProperties f = new ConnectionProperties
                        {

                            FileName = Path.GetFileName(fullname),
                            FilePath = Path.GetDirectoryName(fullname),
                            Ext = Path.GetExtension(fullname),
                            ConnectionName = Path.GetFileName(fullname)
                        };

                        f.Category = DatasourceCategory.VIEWS;
                        f.DriverVersion = "1";
                        f.DriverName = "DataViewReader";

                        DMEEditor.ConfigEditor.DataConnections.Add(f);
                        IDataViewDataSource ds = (IDataViewDataSource)DMEEditor.GetDataSource(f.ConnectionName);
                       
                        if (EntitySource != null)
                        {
                            int x = ds.AddEntitytoDataView(EntitySource.DataSource, EntitySource.BranchText, EntitySource.DataSource.Dataconnection.ConnectionProp.SchemaName, null);
                        }
                        ds.WriteDataViewFile(fullname);
                        DataSource = DMEEditor.GetDataSource(f.ConnectionName);
                        DataView = ds.DataView;
                        DataView.EntityDataSourceID = EntitySource.DataSource.DatasourceName;
                        IBranch br = CreateViewNode(f);


                        if (EntitySource != null)
                        {
                           
                          //  int x = DMEEditor.viewEditor.AddEntitytoDataView(EntitySource.DataSource, EntitySource.BranchText, EntitySource.DataSource.Dataconnection.ConnectionProp.SchemaName, null, DataView.id);
                            br.CreateChildNodes();
                        }
                      
                        // IBranch br = TreeEditor.Treebranchhandler.GetBranch(DataView.ViewName);



                    }
                    DMEEditor.AddLogMessage("Success", "Added View", DateTime.Now, 0, null, Errors.Ok);
                }
                else
                {
                    Visutil.Controlmanager.MsgBox("DM Engine", "Please Try another name . DataSource Exist");
                }

            }
            catch (Exception ex)
            {
                string mes = "Could not Added View ";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
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
                viewbr.GuidID= i.GuidID;
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
        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            DataViewCategoryNode categoryBranch = null;
        try
        {
                categoryBranch = new DataViewCategoryNode(TreeEditor, DMEEditor,this, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, "category.png");
                TreeEditor.Treebranchhandler.AddBranch(this, categoryBranch);
               
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
        #endregion

}
}
