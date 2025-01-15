using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheTechIdea.Beep;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;


namespace TheTechIdea.Beep.TreeNodes.Dev
{
    [AddinAttribute(Caption = "Developer", misc = "DEV", FileType = "Beep", iconimage = "dev.png", menu = "DEV", ObjectType = "Beep", ClassType = "LJ")]
    [AddinVisSchema(BranchType = EnumPointType.Root)]
    public class DevRootBranch : IBranch
    {
        public string MenuID { get; set; }
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public bool IsDataSourceNode { get; set; } = false;
        public DevRootBranch()
        {
            
        }
        public bool Visible { get; set; } = true;

        public int Order { get; set; } = 0;
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int ID { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public EntityStructure EntityStructure { get; set; }
        public int MiscID { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "Developer";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Root;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "dev.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "DEV";
         public IBranch ParentBranch { get  ; set  ; }
        public string ObjectType { get; set; } = "Beep.DEV";
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            CreateNodes();
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
        public IErrorsInfo CreateNodes()
        {

            try
            {
                //CreateFolders();
               // DMEEditor.ConfigEditor.LoadAIScriptsValues();
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                List<AssemblyClassDefinition> ls = DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null).ToList();
                List<AssemblyClassDefinition> aibranchs = ls.Where(p => p.classProperties.menu.Equals("DEV", StringComparison.InvariantCultureIgnoreCase) && p.PackageName != this.Name).ToList();
                foreach (AssemblyClassDefinition item in aibranchs)
                {


                    Type adc = DMEEditor.assemblyHandler.GetType(item.PackageName);
                    // ConstructorInfo ctor = adc.GetConstructors().First();
                    //  ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                    IBranch br = (IBranch)DMEEditor.assemblyHandler.GetInstance(item.PackageName);
                    br.BranchText= item.classProperties.Caption;
                    br.SetConfig(TreeEditor, DMEEditor, this, item.PackageName, 0, EnumPointType.Function, "");
                    TreeEditor.Treebranchhandler.AddBranch(this, br);
                    //int id = TreeEditor.SeqID;
                    //br.Name = item.PackageName;

                    //br.ID = id;
                    //br.BranchID = id;
                    //br.DMEEditor = DMEEditor;
                    //br.TreeEditor = TreeEditor;
                    //br.BranchID = id;
                    //br.ID = id;
                    //br.Visutil = Visutil;
                    //br.DMEEditor = DMEEditor;
                    //TreeEditor.Treebranchhandler.AddBranch(this, br);
                    //TreeEditor.AddBranchToParentInBranchsOnly(this,br);
                    //br.CreateChildNodes();
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
    }
}
