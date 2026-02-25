# DependencyAnalyzer Enhancement Plan

## Current Analysis
The current `DependencyAnalyzer.cs` treats any multiple unique versions as a conflict, which is overly simplistic. The system needs to distinguish between actual compatibility issues and benign version differences.

## Proposed Changes

### 1. Create Severity Enum
- Add `DependencyConflictSeverity` enum with 3 levels:
  - `Error`: Major version conflicts (incompatible APIs)
  - `Warning`: Minor version conflicts (potentially breaking changes)  
  - `Info`: Patch version conflicts (bug fixes only)

### 2. Update DependencyConflict Record
- Add `Severity` property to `DependencyConflict`
- Maintain backward compatibility

### 3. Enhanced Conflict Detection Logic
Replace the simple "unique versions > 1" check with:

**Error Level (Major Version Conflicts):**

- When multiple versions/version ranges exist AND at least one major version is not satisfied by all dependencies
- Examples:
  - [8.0.0, ) and [8.0.0]. First major version which satisfies first range but doesn't satisfy the second one is 9.0.0
  - [5.0.0, 8.0.0) and [6.0.0, ). First major version which doesn't satisfy first range but satisfies the second one is 8.0.0
  - [8.0.0, ) and [8.1.0, ). First major version which satisfies first range but doesn't satisfy the second one is 8.0.0 


**Warning Level (Minor Version Conflicts):**  

- When no major conflicts exist BUT at least one minor version is not satisfied by all dependencies
- Examples:
  - [8.0.0, 8.15.0] and [8.0.0, 8.5.10]. There is no single major version which would not satisfy any range, however the first version which satisfied first range but doesn't satisfy the second one is 8.6.0.


**Info Level (Patch Version Conflicts):**

- When no major/minor conflicts exist BUT patch versions differ
- Examples:
  - [8.0.0, 8.15.2] and [8.0.0, 8.15.10]. There are no either major not minor version which wouldn't satisfy both ranges. However the first version which doesn't satisfy the first range but does satify the second one is 8.15.3.


### 4. Implementation Details
- Add version compatibility checking methods to analyze semantic versioning rules
- Utilize existing `DependencyVersion` and `DependencyVersionRange` classes
- Leverage NuGet's `NuGetVersion` for proper semantic version comparison
- Update `FindVersionConflicts` method to return conflicts with appropriate severity levels

### 5. Files to Modify
- `/src/Fink.Abstractions/DependencyAnalyzer.cs` - Main logic
- `/src/Fink.Abstractions/DependencyConflict.cs` - Add severity property
- Add new `DependencyConflictSeverity.cs` enum

This approach provides meaningful conflict categorization while maintaining the existing API structure.