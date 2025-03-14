﻿using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.TreeNodes
{
    [AddinAttribute(Caption = "Admin.", BranchType = EnumPointType.Genre, misc = "Beep", FileType = "Beep", iconimage = "configadmin.png", menu = "configadmin", ObjectType = "Beep",order =99999, ClassType = "LJ")]
    [AddinVisSchema(BranchType = EnumPointType.Genre, BranchClass = "Genre",Order =9999)]
    public class ConfigGenereNode : IBranch
    {
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public bool Visible { get; set; } = true;

        public ConfigGenereNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {

            BranchText = "Admin.";
            BranchClass = "CONFIGADMIN.ROOT";
            IconImageName = "configadmin.png";
            BranchType = EnumPointType.Genre;
            ID = pID;

        }
        public ConfigGenereNode()
        {
            BranchText = "Admin.";
            BranchClass = "CONFIGADMIN.ROOT";
            IconImageName = "configadmin.png";
            BranchType = EnumPointType.Genre;
            ID = -1;
        }
        public string MenuID { get; set; }
        public bool IsDataSourceNode { get; set; } = false;
        public string ObjectType { get; set; } = "Beep";
        public int Order { get; set; } = 0;
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int ID { get; set; } = -1;
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public EntityStructure EntityStructure { get; set; }
        public int MiscID { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "Admin.";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Genre;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "configadmin.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "CONFIGADMIN.GENRE";
         public IBranch ParentBranch { get  ; set  ; }
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
          ;return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
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
    }
}
