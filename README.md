# Beep.Branchs

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)  
**Current Status: Alpha** - Actively developed, expect bugs, contributions welcome!

`Beep.Branchs` is a component of the Beep Data Management Engine (BeepDM) framework, providing tree node implementations (`IBranch`) for visualizing and interacting with various data source categories in a hierarchical structure. These nodes integrate with BeepDM’s visualization system (`IVisManager`) and configuration management (`IConfigEditor`), enabling users to manage data sources such as relational databases (RDBMS), NoSQL databases, files, data views, and reports.

## Overview

This repository contains C# classes that implement the `IBranch` interface, representing different types of nodes in a tree structure. Each node type corresponds to a specific data source category or functional role, such as root nodes, category nodes, data points, or entities. These nodes are used within BeepDM to build a navigable tree view of data sources and their associated entities.

### Key Features
- **Hierarchical Structure**: Supports root, category, data point, and entity nodes for organizing data sources.
- **Extensible**: Nodes are add-ins (`[AddinAttribute]`) that can be customized or extended.
- **Integration**: Works with `IDMEEditor`, `IConfigEditor`, and `IDataSource` for configuration and data access.
- **Commands**: Provides exposed methods (via `[CommandAttribute]`) for user interactions like creating, editing, or refreshing nodes.

## Branch Node Categories

### 1. Data Views (`TreeNodes.DataViews`)
Nodes for managing federated data views across multiple data sources.

- **`DataViewRootNode`**: Root node for all data views.
  - **Attributes**: `[AddinAttribute(Caption = "DataView", BranchType = Root)]`
  - **Commands**: `CreateView`, `AddViewFile`, `CreateView(IBranch)`
  - **Purpose**: Manages top-level data view operations and creates child nodes for views or categories.

- **`DataViewCategoryNode`**: Category node grouping data views.
  - **Attributes**: `[AddinAttribute(Caption = "DataView", BranchType = Category)]`
  - **Purpose**: Organizes data views under a category, creating child `DataViewNode` instances.

- **`DataViewNode`**: Represents an individual data view.
  - **Attributes**: `[AddinAttribute(Caption = "DataView", BranchType = DataPoint)]`
  - **Commands**: `Edit`, `CreateViewEntites`, `SaveView`, `RemoveView`, `CreateComposedLayer`, `ClearView`
  - **Purpose**: Manages a specific data view, including editing and entity management.

- **`DataViewEntitiesNode`**: Represents entities within a data view.
  - **Attributes**: `[AddinAttribute(Caption = "DataView", BranchType = Entity)]`
  - **Commands**: `EditEntity`, `LinkEntity`, `RemoveEntity`, `GetChilds`, `RemoveChilds`, `DataEdit`, `FieldProperties`
  - **Purpose**: Handles entity-specific operations within a data view.

### 2. Files (`TreeNodes.Files`)
Nodes for managing file-based data sources.

- **`FileRootNode`**: Root node for file-based data sources.
  - **Attributes**: `[AddinAttribute(Caption = "Files", BranchType = Root)]`
  - **Commands**: `AddFile`, `AddFolder`
  - **Purpose**: Top-level node for file operations, creating file or category nodes.

- **`FileCategoryNode`**: Category node for grouping files.
  - **Attributes**: `[AddinAttribute(Caption = "Files", BranchType = Category)]`
  - **Commands**: `AddFile`, `AddFolder`
  - **Purpose**: Organizes files under categories.

- **`FileEntityNode`**: Represents a file data source.
  - **Attributes**: `[AddinAttribute(Caption = "Files", BranchType = DataPoint)]`
  - **Commands**: `GetSheets`, `RefreshSheets`, `EditFileConnection`
  - **Purpose**: Manages a specific file, creating child sheet nodes.

- **`FileEntitySheetNode`**: Represents a sheet within a file (e.g., Excel).
  - **Attributes**: `[AddinAttribute(Caption = "Files", BranchType = Entity)]`
  - **Purpose**: Displays individual sheets within a file.

- **`FileFolderNode`**: Represents a folder in the file hierarchy.
  - **Attributes**: `[AddinAttribute(Caption = "Folder", BranchType = Function)]`
  - **Purpose**: Placeholder for folder navigation (minimal implementation).

### 3. RDBMS (`TreeNodes.RDBMS`)
Nodes for managing relational database management systems.

- **`DatabaseRootNode`**: Root node for RDBMS data sources.
  - **Attributes**: `[AddinAttribute(Caption = "RDBMS", BranchType = Root)]`
  - **Commands**: `CreateNewLocalDatabase`, `NewDBConnection`
  - **Purpose**: Manages RDBMS connections at the top level.

- **`DatabaseCategoryNode`**: Category node for grouping RDBMS connections.
  - **Attributes**: `[AddinAttribute(Caption = "RDBMS", BranchType = Category)]`
  - **Purpose**: Organizes RDBMS connections under categories.

- **`DatabaseNode`**: Represents an RDBMS connection.
  - **Attributes**: `[AddinAttribute(Caption = "RDBMS", BranchType = DataPoint)]`
  - **Commands**: `EditDBConnection`, `GetDatabaseEntites`, `RefreshDatabaseEntites`
  - **Purpose**: Manages a specific RDBMS connection, creating entity nodes.

- **`DatabaseEntitesNode`**: Represents entities (tables) within an RDBMS.
  - **Attributes**: `[AddinAttribute(Caption = "RDBMS", BranchType = Entity)]`
  - **Purpose**: Displays individual tables or entities.

### 4. Reports (`TreeNodes.Reports`)
Nodes for managing report definitions.

- **`QueryRootNode`**: Root node for reports (mislabeled; intended as `ReportRootNode`).
  - **Attributes**: `[AddinAttribute(Caption = "Reports", BranchType = Root)]`
  - **Commands**: `CreateReport`
  - **Purpose**: Manages report-related operations at the top level.

- **`ReportCategoryNode`**: Category node for grouping reports.
  - **Attributes**: `[AddinAttribute(Caption = "Reports", BranchType = Category)]`
  - **Purpose**: Organizes reports under categories.

- **`ReportNode`**: Represents an individual report.
  - **Attributes**: `[AddinAttribute(Caption = "Reports", BranchType = Function)]`
  - **Commands**: `EditReport`, `CreateReport`, `GenerateReport`, `Remove`
  - **Purpose**: Manages a specific report, including editing and generation.

### 5. NoSQL (`TreeNodes.NoSQL`)
Nodes for managing NoSQL data sources.

- **`NoSqlRootNode`**: Root node for NoSQL data sources.
  - **Attributes**: `[AddinAttribute(Caption = "NoSQL", BranchType = Root)]`
  - **Commands**: `NewNOSQLConnection`
  - **Purpose**: Manages NoSQL connections at the top level.

- **`NoSqlCategoryNode`**: Category node for grouping NoSQL connections.
  - **Attributes**: `[AddinAttribute(Caption = "NoSQL", BranchType = Category)]`
  - **Purpose**: Organizes NoSQL connections under categories.

- **`NoSqlSourceNode`**: Represents a NoSQL data source.
  - **Attributes**: `[AddinAttribute(Caption = "NoSQL", BranchType = DataPoint)]`
  - **Commands**: `CreateDatabaseEntites`, `EditNOSQLConnection`
  - **Purpose**: Manages a specific NoSQL connection, creating entity nodes.

- **`NoSqlEntityNode`**: Represents entities within a NoSQL data source.
  - **Attributes**: `[AddinAttribute(Caption = "NoSQL", BranchType = Entity)]`
  - **Purpose**: Displays individual entities (e.g., collections).

## Usage

These branch nodes are integrated into BeepDM via the `ITree` and `IVisManager` components. To use them:

1. **Initialization**: Nodes are instantiated by BeepDM’s tree editor (`ITree`) based on configurations in `IConfigEditor`.
2. **Configuration**: Use `SetConfig` to initialize each node with `ITree`, `IDMEEditor`, and parent node details.
3. **Population**: Call `CreateChildNodes` to populate the tree with child nodes based on data source configurations.
4. **Interaction**: Use exposed commands (e.g., `Edit`, `GetEntities`) to interact with nodes via the UI.

### Example: Adding a File Node
```csharp
var rootNode = new FileRootNode(treeEditor, dmeEditor, null, "Files", treeEditor.SeqID, EnumPointType.Root, "file.png", null);
rootNode.CreateChildNodes(); // Populates with FileCategoryNode or FileEntityNode
rootNode.AddFile(); // Triggers file addition UI
