# TheTechIdea.Beep.TreeNodes

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Version](https://img.shields.io/badge/version-1.0.15-green.svg)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-purple.svg)

## Overview

TheTechIdea.Beep.TreeNodes is a comprehensive tree node library for the Beep Enterprise framework. It provides a rich collection of tree node implementations for various data sources, file systems, development tools, and enterprise features.

## Features

- **??? File Management** - File and folder tree nodes with support for various file types
- **??? Database Support** - RDBMS and NoSQL database tree nodes
- **?? Web API Integration** - REST API and web service nodes
- **?? Reporting** - Report management and organization nodes
- **?? Configuration** - System and application configuration nodes
- **?? Workflow** - Business process and workflow nodes
- **??? Project Management** - Development project organization
- **?? DDL Management** - Database schema and DDL operation nodes
- **?? Data Mapping** - Entity and data mapping nodes
- **?? Cloud Services** - Cloud platform integration nodes
- **?? AI Integration** - AI and machine learning service nodes

## Architecture

### Core Components

#### IBranch Interface
All tree nodes implement the `IBranch` interface which provides:
- Hierarchical structure management
- Context menu actions
- Visual representation
- Data source integration

#### Base Node Types
- **Root Nodes** - Top-level container nodes
- **Category Nodes** - Organizational grouping nodes  
- **Entity Nodes** - Data entity representation nodes
- **Action Nodes** - Executable operation nodes

### Node Categories

```
TheTechIdea.Beep.TreeNodes/
??? AI/                    # AI and ML service nodes
??? Cloud/                 # Cloud platform nodes
??? Config/                # Configuration management
??? DDL/                   # Database DDL operations
??? Dev/                   # Development tools
??? Files/                 # File system management
??? Mapping/               # Data mapping and transformation
??? NoSQL/                 # NoSQL database nodes
??? Project/               # Project management
??? RDBMS/                 # Relational database nodes
??? Reports/               # Reporting and analytics
??? WebAPI/                # Web API integration
??? WorkFlow/              # Business process workflows
```

## Quick Start

### Installation

```xml
<PackageReference Include="TheTechIdea.Beep.TreeNodes" Version="1.0.15" />
```

### Basic Usage

```csharp
// Create a data sources root node
var dataSourcesRoot = new DataSourcesRootNode();
dataSourcesRoot.SetConfig(treeEditor, dmeEditor, parentNode, "Data Sources", 1, EnumPointType.Genre, "datasources.png");

// Create a database node
var dbNode = new DatabaseNode(treeEditor, dmeEditor, dataSourcesRoot, "MyDatabase", 2, EnumPointType.DataPoint, "database.png", dataSource);

// Add to tree
treeEditor.Treebranchhandler.AddBranch(dataSourcesRoot, dbNode);
```

### Node Implementation Pattern

```csharp
[AddinAttribute(Caption = "My Node", misc = "Beep", BranchType = EnumPointType.Entity, iconimage = "mynode.png")]
public class MyCustomNode : IBranch
{
    public string BranchText { get; set; }
    public EnumPointType BranchType { get; set; }
    public string IconImageName { get; set; }
    
    // Implement required IBranch members
    public IErrorsInfo CreateChildNodes() { /* Implementation */ }
    public IErrorsInfo ExecuteBranchAction(string ActionName) { /* Implementation */ }
    // ... other interface members
}
```

## Node Types Reference

### File System Nodes
- **FileRootNode** - Root container for file operations
- **FileCategoryNode** - File type categorization
- **FileEntityNode** - Individual file representation
- **FileFolderNode** - Directory structure
- **FileEntitySheetNode** - Spreadsheet/tabular file data

### Database Nodes
- **DatabaseRootNode** - Database system root
- **DatabaseCategoryNode** - Database grouping
- **DatabaseNode** - Individual database
- **DatabaseEntitiesNode** - Database table/collection

### Project Management
- **ProjectRootBranch** - Project system root
- **ProjectProjectNode** - Individual project
- **ProjectFolderNode** - Project directory
- **ProjectFileNode** - Project file

### Configuration
- **ConfigRootNode** - Configuration system root
- **ConfigCategoryNode** - Configuration grouping
- **ConfigEntityNode** - Configuration item

## Advanced Features

### Custom Actions
Nodes support custom context menu actions via the `CommandAttribute`:

```csharp
[CommandAttribute(Caption = "Custom Action", iconimage = "action.png")]
public IErrorsInfo CustomAction()
{
    // Implementation
    return DMEEditor.ErrorObject;
}
```

### Visual Styling
Each node type supports custom icons and visual representations through embedded resources in the `GFX/` folder.

### Data Source Integration
Nodes can be bound to various data sources:
- RDBMS (SQL Server, PostgreSQL, MySQL, etc.)
- NoSQL (MongoDB, DuckDB, etc.)
- Files (CSV, JSON, XML, Excel, etc.)
- Web APIs (REST, GraphQL, etc.)
- Cloud Services (AWS, Azure, GCP, etc.)

## Dependencies

- **TheTechIdea.Beep.DataManagementEngine** (?2.0.43) - Core data management
- **TheTechIdea.Beep.Vis.Modules** (?2.0.24) - Visual framework components

## Supported Platforms

- .NET 8.0
- .NET 9.0
- Windows, Linux, macOS

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the existing node implementation patterns
4. Add appropriate icons to the `GFX/` folder
5. Include unit tests
6. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Documentation

For detailed documentation and examples, see:
- [HOWTO.md](HOWTO.md) - Implementation guide
- [Beep Framework Documentation](https://github.com/The-Tech-Idea/BeepDM)

## Support

- GitHub Issues: [Report bugs and feature requests](https://github.com/The-Tech-Idea/Beep.Branchs/issues)
- Community: [Beep Framework Discussions](https://github.com/The-Tech-Idea/BeepDM/discussions)

---

**The Tech Idea** - Building enterprise solutions with the Beep Framework