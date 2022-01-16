# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.1] - 2021-09-23
### Added
- Added Appliers which function like observers, but in reverse. They can be used to update an observer.
- Added TextFormattingProvider that can be used to add new formatting types to existing TextDetailBinders
- Added simple TimespanFormattingProvider to function as example usage.

## Changed
- Changed the single state binder invert flag to enum for better readablity in editor.
- Added a new apply at start boolean to single state binder to make it applying changes at start up.
- Removed the UserContainer.prefab and updated the sample scene with new usages.

### Fixed
- Removed old Editor folder meta file from the Data/Serialized directory
- Fixed issue where adding new details would throw if not including addressables.
- Fixed issue where the scene meta would conflict with default unity sample scene.
- Fixed issue where single state binder would not initialize properly at startup from default references.

## [1.0.0] - 2021-09-20
### Changed
- Initial Release

## [1.0.2] - 2022-01-11
### Fixed
- Fixed issue where conditions for enums would initialize as two parameter values instead of value and parameter

## [1.0.3] - 2022-01-16
### Fixed
- Fixed null reference during comparisons in the base detail if the value is null.