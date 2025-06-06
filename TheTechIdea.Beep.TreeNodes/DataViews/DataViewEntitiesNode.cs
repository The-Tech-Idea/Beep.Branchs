﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.DataView;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    [AddinAttribute(Caption = "DataView", BranchType = EnumPointType.Entity, Name = "DataViewEntitiesNode.Beep", misc = "Beep", iconimage = "dataview.png", menu = "Beep", ObjectType = "Beep")]
    public class DataViewEntitiesNode : IBranch 
    {
        public DataViewEntitiesNode()
        {

        }
        public DataViewEntitiesNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string pDSName, EntityStructure entityStructure)
        {



            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode.BranchID;

            BranchType = pBranchType;
            IconImageName = pimagename;

            ds = (IDataViewDataSource)DMEEditor.GetDataSource(pDSName);
            DataView = ds.DataView;
            EntityStructure = entityStructure;
            if (string.IsNullOrEmpty(entityStructure.Caption) || string.IsNullOrWhiteSpace(entityStructure.Caption))
            {
                entityStructure.Caption = entityStructure.EntityName;
            }
            if (string.IsNullOrEmpty(entityStructure.DatasourceEntityName) || string.IsNullOrWhiteSpace(entityStructure.DatasourceEntityName))
            {
                entityStructure.DatasourceEntityName = entityStructure.EntityName;
            }
            BranchText = entityStructure.Caption;
            MiscID = entityStructure.Id;
            DataSourceName = entityStructure.DataSourceID; //entityStructure.DataSourceID;
           
            if (pID != 0)
            {
                ID = pID;
                BranchID = pID;
            }
            Name=entityStructure.EntityName.Trim();
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
        public string Name { get; set; }
        public string BranchText { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
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
        public  IAppManager  Visutil { get; set; }
        public int MiscID { get; set; }
        IDataViewDataSource ds;
        public IDMDataView DataView
        {
            get
            {
                return ds.DataView;
            }
            set
            {
                ds.DataView = value;
            }
        }
        int DataViewID
        {
            get
            {
                return ds.DataView.ViewID;
            }
            set
            {
                ds.DataView.ViewID = value;
            }
        }
      
        #endregion "Properties"
        #region "Interface Methods"
     
        private void GetChildNodes(List<EntityStructure> Childs,EntityStructure Parent, IBranch ParentBranch)
        {

            DataViewEntitiesNode dbent = null ;
           
            foreach (EntityStructure i in Childs)
            {
                IBranch branch = TreeEditor.Treebranchhandler.GetBranchByMiscID(i.Id);
                if (branch == null)
                {
                   
                    dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, ParentBranch, i.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(i.Viewtype),ds.DatasourceName, i);
                    TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                    TreeEditor.Treebranchhandler.AddBranch(ParentBranch, dbent);
                    dbent.CreateChildNodes();
                  
                }
                else
                {
                    if (!ChildBranchs.Where(x => x.BranchText == i.EntityName).Any())
                    {
                       
                        dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, ParentBranch, i.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(i.Viewtype), ds.DatasourceName, i);
                        TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                        TreeEditor.Treebranchhandler.AddBranch(ParentBranch, dbent);
                        dbent.CreateChildNodes();
                      
                    }
                    else
                    {
                        dbent =(DataViewEntitiesNode) branch;
                    }

                }
                List<EntityStructure> otherchilds = DataView.Entities.Where(cx => (cx.Id != i.Id) && (cx.ParentId == i.Id)).ToList();
                if (otherchilds != null)
                {
                    if (otherchilds.Count > 0)
                    {
                        GetChildNodes(otherchilds, i, dbent);
                    }
                }

            }
        }
        public IErrorsInfo CreateChildNodes()
        {

            try
            {
                IBranch dbent;
                List<EntityStructure> cr = DataView.Entities.Where(cx => (cx.Id != EntityStructure.Id) && (cx.ParentId == EntityStructure.Id) ).ToList();
                foreach (EntityStructure i in cr)
                {
                    //if (ChildBranchs.Count == 0)
                    //{
                        if (ChildBranchs.Where(x => x.BranchText == i.EntityName).Any() == false)
                        {

                            dbent = new DataViewEntitiesNode(TreeEditor, DMEEditor, this, i.EntityName, TreeEditor.SeqID, EnumPointType.Entity, ds.GeticonForViewType(i.Viewtype), ds.DatasourceName, i);
                            
                            TreeEditor.AddBranchToParentInBranchsOnly(this,dbent);
                           TreeEditor.Treebranchhandler.AddBranch(this, dbent);
                           dbent.CreateChildNodes();
                         

                    }
                        else
                        {
                            dbent = ChildBranchs.Where(x => x.BranchText == i.EntityName).FirstOrDefault();
                            dbent.CreateChildNodes();
                        }

                    List<EntityStructure> childs = DataView.Entities.Where(cx => (cx.Id != i.Id) && (cx.ParentId == i.Id)).ToList();
                    if (childs != null)
                    {
                        if (childs.Count > 0)
                        {
                            GetChildNodes(childs, i, this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Create Child Node";
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

            try
            {
                if (Visutil.DialogManager.InputBoxYesNo("DM Engine","Are you sure you want to remove Entities?")==BeepDialogResult.Yes)
                {
                    foreach (IBranch item in ChildBranchs)
                    {
                        TreeEditor.Treebranchhandler.RemoveBranch(item);
                        ds.RemoveEntity(EntityStructure.Id);
                        // DMEEditor.viewEditor.Views.Where(x => x.ViewName == DataView.ViewName).FirstOrDefault().Entity.Remove(EntityStructure);
                    }


                    DMEEditor.AddLogMessage("Success", "Removed Branch Successfully", DateTime.Now, 0, null, Errors.Ok);
                }
               
            }
            catch (Exception ex)
            {
                string mes = "Could not Removed Branch Successfully";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
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
        private List<ObjectItem> Createlistofitems()
        {
            List<ObjectItem> ob = new List<ObjectItem>(); 
            ObjectItem it = new ObjectItem();
            it.obj = this;
            it.Name = "Branch";
            ob.Add(it);
            ObjectItem EntityStructureit = new ObjectItem();
            EntityStructureit.obj = EntityStructure;
            EntityStructureit.Name = "EntityStructure";
            ob.Add(EntityStructureit);
            return ob;
        }
        [CommandAttribute(Caption = "Edit", Hidden = false, iconimage = "edit_entity.png")]
        public IErrorsInfo EditEntity()
        {

            try
            {
                EntityStructure = ds.Entities[ds.Entities.FindIndex(o => o.Id == EntityStructure.Id)];
                string[] args = { "New View", null, null };
              
                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = DataView,
                    CurrentEntity = EntityStructure.DatasourceEntityName,
                    Id = BranchID,
                    ObjectType = "VIEWENTITY",
                    DataSource = DataSource,
                    ObjectName = DataView.ViewName,
                    Objects = Createlistofitems(),
                    DatasourceName = EntityStructure.DataSourceID,
                    EventType = "VIEWENTITY"

                };
                Passedarguments.Objects.Add(new ObjectItem() { Name = "TitleText", obj = $"Edit {EntityStructure.EntityName}" });
                Visutil.ShowPage("Uc_DataViewEntityEditor", Passedarguments);


              
                DMEEditor.AddLogMessage("Success", "Edit Control Shown", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not show Edit Control";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Link", Hidden = false, iconimage = "linkicon.png")]
        public IErrorsInfo LinkEntity()
        {

            try
            {
                EntityStructure = ds.Entities[ds.Entities.FindIndex(o => o.Id == EntityStructure.Id)];
                string[] args = { "New View", null, null };
                if (string.IsNullOrEmpty(EntityStructure.DatasourceEntityName))
                {
                    EntityStructure.DatasourceEntityName = EntityStructure.EntityName;
                }
                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = DataView,
                    CurrentEntity = EntityStructure.DatasourceEntityName,
                    Id = BranchID,
                    ObjectType = "LINKENTITY",
                    DataSource = DataSource,
                    ObjectName = DataView.ViewName,
                    Objects = Createlistofitems(),
                    DatasourceName = EntityStructure.DataSourceID,
                    EventType = "LINKENTITY"

                };
                Passedarguments.Objects.Add(new ObjectItem() { Name = "TitleText", obj = $"Link {EntityStructure.EntityName}" });
                Visutil.ShowPage("uc_linkentitytoanother", Passedarguments);



                DMEEditor.AddLogMessage("Success", "Edit Control Shown", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not show Edit Control";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Remove", Hidden = false, iconimage = "remove.png")]
        public IErrorsInfo RemoveEntity()
        {

            try
            {
                string[] args = { "New View", null, null };
               
                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = DataView,
                    CurrentEntity = EntityStructure.DatasourceEntityName,
                    Id = BranchID,
                    ObjectType = "VIEWENTITY",
                    DataSource = DataSource,
                    ObjectName = DataView.ViewName,
                    Objects = Createlistofitems(),
                    EventType = "REMOVEENTITY"

                };
                if (Visutil.DialogManager.InputBoxYesNo("DM Engine","Are you sure you want to remove Entity?") == BeepDialogResult.Yes)
                {
                    TreeEditor.Treebranchhandler.RemoveBranch(this);
                    //---- Remove From View ---- //
                    ds.RemoveEntity(EntityStructure.Id);
                    DMEEditor.AddLogMessage("Success", "Removed Entity Node", DateTime.Now, 0, null, Errors.Ok);
                }
               
                
             //   ActionNeeded?.Invoke(this, Passedarguments);
               
            }
            catch (Exception ex)
            {
                string mes = "Could not Entity Node";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Get Childs", Hidden = false, iconimage = "get_childs.png")]
        public IErrorsInfo GetChilds()
        {

            try
            {
                DataSource = DMEEditor.GetDataSource(EntityStructure.DataSourceID);
                if (DataSource!=null)
                {

                    ds.GenerateDataViewForChildNode(DataSource, EntityStructure.Id, EntityStructure.DatasourceEntityName, EntityStructure.SchemaOrOwnerOrDatabase, "");
                    CreateChildNodes();
                    DMEEditor.AddLogMessage("Success", "Got child Nodes", DateTime.Now, 0, null, Errors.Ok);
                }else
                {
                    Visutil.DialogManager.MsgBox("DM Engine", "Couldnot Get DataSource For Entity");
                }
             
            }
            catch (Exception ex)
            {
                string mes = "Could not get child nodes";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Caption = "Remove Childs", Hidden = false, iconimage = "remove_childs.png")]
        public IErrorsInfo RemoveChilds()
        {

            try
            {
                if (Visutil.DialogManager.InputBoxYesNo("DM Engine","Are you sure you want to remove child  Entities?")==BeepDialogResult.Yes)
                {
                    TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                    ds.RemoveChildEntities(EntityStructure.Id);
                    DMEEditor.AddLogMessage("Success", "Removed Child Entites", DateTime.Now, 0, null, Errors.Ok);
                }
              
            }
            catch (Exception ex)
            {
                string mes = "Could not Remove Child Entites";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
       
        [CommandAttribute(Caption = "Data Edit", Hidden = false,iconimage = "editentity.png")]
        public IErrorsInfo DataEdit()
        {

            try
            {

                EntityStructure = ds.Entities[ds.Entities.FindIndex(o => o.Id == EntityStructure.Id)];
               

                string[] args = { "New View", null, null };
                   
                    PassedArgs Passedarguments = new PassedArgs
                    {
                        Addin = null,
                        AddinName = null,
                        AddinType = "",
                        DMView = DataView,
                        CurrentEntity = EntityStructure.DatasourceEntityName,
                        Id = BranchID,
                        ObjectType = "VIEWENTITY",
                        DataSource = DataSource,
                        ObjectName = DataView.ViewName,
                        Objects = Createlistofitems(),
                        DatasourceName = EntityStructure.DataSourceID,
                        EventType = "CRUDENTITY"

                    };

                    Visutil.ShowPage("crudmanager",    Passedarguments);

                

                //  DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {                string mes = "Could not Edit Entity";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
   
        [CommandAttribute(Caption = "Field Properties", iconimage = "properties.png")]
        public IErrorsInfo FieldProperties()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //   DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                string[] args = { "New Query Entity", null, null };
               


                PassedArgs Passedarguments = new PassedArgs
                {
                    Addin = null,
                    AddinName = null,
                    AddinType = "",
                    DMView = DataView,
                    CurrentEntity = EntityStructure.EntityName,
                    Id = ID,
                    ObjectType = "ENTITY",
                    DataSource = DataSource,
                    ObjectName = DataView.ViewName,

                    Objects = Createlistofitems(),

                    DatasourceName = DataView.DataViewDataSourceID,
                    EventType = "ENTITY"

                };
                Passedarguments.Objects.Add(new ObjectItem() { Name = "TitleText", obj = $"Fields for {EntityStructure.EntityName}" });
                Visutil.ShowPage("uc_fieldproperty", Passedarguments);



            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in Filling Database Entites ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }

        public  IBranch  CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }

        #endregion Exposed Interface"
        #region "Other Methods"

        #endregion"Other Methods"
    }
}
