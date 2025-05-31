---
name: Bug Report
about: Create a report to help me improve Fink
title: '[BUG] '
labels: ['bug', 'needs-triage']
assignees: ''
---

## 🐛 Bug Report

### **Describe the bug**
<!-- A clear and concise description of what the bug is. -->

### **To Reproduce**
Steps to reproduce the behavior:
1. Run Fink with command: `dotnet run --project src/Fink/Fink.csproj [project-path] [target-framework]`
2. Project type: <!-- e.g., .NET 9.0 web app -->
3. See error

**Command used:**
```bash
# Paste the exact command you ran
```

### **Expected behavior**
<!-- A clear and concise description of what you expected to happen. -->

### **Actual behavior**
<!-- A clear and concise description of what actually happened. -->

**Error output:**
```
# Paste any error messages or unexpected output here
```

## 🔍 Environment Information

### **Fink Version**
- [ ] Latest from master branch
- [ ] Release version: <!-- e.g., v1.0.0 -->
- [ ] Commit SHA: <!-- if using development version -->

### **System Information**
- **OS**: <!-- e.g., Windows 11, macOS 14.1, Ubuntu 22.04 -->
- **Architecture**: <!-- e.g., x64, ARM64 -->
- **.NET SDK Version**: <!-- Run `dotnet --version` -->

### **Project Under Analysis**
- **Project Type**: <!-- e.g., Console App, Web API, Class Library -->
- **Target Framework(s)**: <!-- e.g., net9.0, net8.0, net48 -->
- **Package Management**: 
  - [ ] PackageReference (SDK-style)
  - [ ] packages.config (legacy)
  - [ ] Mixed
- **Multi-targeting**: 
  - [ ] Single target framework
  - [ ] Multi-targeting project

## 🔧 Analysis Results

### **Build Status**
- [ ] Project builds successfully outside of Fink
- [ ] Project fails to build outside of Fink
- [ ] Fink build succeeds but analysis fails
- [ ] Fink build fails

### **Lock File Information**
- **Lock file exists**: 
  - [ ] Yes, at expected location
  - [ ] No, missing
  - [ ] Unknown/Not checked
- **Lock file path**: <!-- e.g., /path/to/project/obj/project.assets.json -->

### **Dependency Analysis**
<!-- If Fink runs but produces unexpected results -->
- **Expected number of dependencies**: 
- **Actual number reported by Fink**: 
- **Expected conflicts**: 
- **Actual conflicts reported**: 

## 📋 Additional Context

### **Workarounds**
<!-- Any workarounds you've found -->

### **Related Issues**
<!-- Link to any related issues -->

### **Screenshots/Logs**
<!-- Add any screenshots, build logs, or additional output that might help -->

**Build log** (if applicable):
```
# Paste relevant parts of build.log or console output
```

**Stack trace** (if applicable):
```
# Paste any exception stack traces
```

---

**Checklist before submitting:**
- [ ] I have searched existing issues for similar problems
- [ ] I have provided all the requested information above
- [ ] I have tested with the latest version of Fink
- [ ] I can reproduce this issue consistently
