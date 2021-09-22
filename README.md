# Owl and Jackalope UX

Owl and Jackalope UX is a simple data binding and UX prototyping tool designed to make iterating on UI screens without needing to write code to back up the original models. It also includes several quality of life features intended to make reusing data models and UX states easier.

![simple_example](https://user-images.githubusercontent.com/7310389/134276910-f89ddd49-0d1b-4dcd-9a11-ea8279499ad3.gif)

![longer_example](https://user-images.githubusercontent.com/7310389/134280647-1ac61ad6-4c9a-442b-ba7c-4549ef0ac2bf.gif)

## Features
1. Create references of data models to use during prototyping without needing to load rest of game.
2. Display live data in editor for easy debugging and allow for manipulation to test responsiveness of scenes.
3. Establish two way data binding with Unity GUI elements.
4. Maintain reusable states defined from conditions met by defined data models to control UX flow without code.
5. Inject actual data models at runtime based on your personal project needs.

## Installation
Owl and Jackalope UX can be installed via the Unity package manager.

or by adding

```"com.owlandjackalope.ux": "https://github.com/AnonyBob/OwlAndJackalope.UX.git"```

to the Packages/manifest.json file

## Usage
### References
References form the backbone of this plugin. A ```IReference``` is simply a collection of ```IDetails```. Each ```IDetail``` is a singular piece of data. Each ```IReference``` can have an arbitrary number of ```IDetails``` each retrievable by a unique string name. When the contents of a ```IDetail``` change it will send out a C# event that can be used to trigger updates in other ```IDetails``` or within the Scene. To create new References you can implement the ```IReference``` interface and expose ```IDetails``` in a manner that fits the need of your project, you can use the ```BaseReference``` which will maintain the ```IDetails``` in a simple Dictionary, or you can simply define them within the Unity Editor by using the Reference Module or Reference Templates. ```IReferences``` were designed to represent models that are assembled using composition and can be thought of as simple key value stores.
#### Reference Module
![Screenshot_092121_084611_PM](https://user-images.githubusercontent.com/7310389/134270466-95fed043-2167-4509-8dcc-52914dc8ddc8.jpg)

```ReferenceModules``` are the in game access points for ```IReferences``` and should be your starting point when creating new user experiences. They should be placed at the top of the transform hierarchy as ```DetailBinders``` will use their parents in order to find the ```ReferenceModules``` for binding. ```ReferenceModules``` allow you to define a Reference that can be referenced in the editor while setting up binders in the Scene. You can create ```IDetails``` with a unique name and value. It supports most primitive types and a couple of commonly used Unity types like Gameobjects, Vector3, and Color as well as Lists and Dictionaries of those types. When you enter playmode, these values will now update to reflect their current values and can be updated through the editor to drive further changes in the UI.
#### Detail Binders
![Screenshot_092121_085132_PM](https://user-images.githubusercontent.com/7310389/134270892-768eca62-6f42-46c5-8edd-f760d0fc1c21.jpg)

```DetailBinders``` are the simple components that can be attached to various elements in the UI that respond to changes in a specified detail and update the resulting visuals using some custom code per Binder definition. A few commonly used binders such as the ```TextDetailBinder``` have already been added that allow you to construct strings out of a collection of details like shown above. Some things may require the creation of a new ```DetailBinders``` in order for you to properly bind changes the UI.

![Screenshot_092121_090346_PM](https://user-images.githubusercontent.com/7310389/134271946-3bebd101-3d3d-4fd5-ae39-87a0479da3ac.jpg)

New Binders can be created by extending the ```BaseDetailBinder```. Each Binder can have multiple ```DetailObservers```. ```DetailObservers``` are wrappers that point to the ```IDetails``` exposed in the ```ReferenceModule```. This wrapper maintains access to the correct ```IDetail``` even if the the Reference in the ```ReferenceModule``` were to change via code in some way. Each ```IDetailObserver``` can be typed or untyped. By adding the ```DetailType``` attribute you can define which types of ```IDetails``` you would like to display in the Editor dropdown. Below you can see an example of how one might bind to multiple different types. In this case, all numeric types supported by the ```ReferenceModule```.

![Screenshot_092121_091144_PM](https://user-images.githubusercontent.com/7310389/134272637-28da1699-b8f6-42bb-8a76-c2c5de369542.jpg)

Each ```IDetailObserver``` is initialized using the Reference provided by the associated ```ReferenceModule``` and a Handler method. The Handler method will run everytime there is a change to the observed detail. This is where the code that responds to the data changes will run. In the below example, whenever the color changes we will set the color on the graphic to the new color defined in the observed ```IDetail```.

![Screenshot_092121_091551_PM](https://user-images.githubusercontent.com/7310389/134273024-5d7d9abd-c1db-405e-bfcd-b9eaa7a881d1.jpg)

***BaseDetailBinder handles IDetail name changes, fetching the Reference Module, and ensuring that the DetailObservers are properly disposed on destruction. If you do opt to create your Binder from scratch it is important that you ensure that all of these things are done as well for the best usage of this tool.***

#### Reference Providers
```ReferenceProviders``` are components that provide a Reference to an existing ```ReferenceModule``` at start up. Up to this point, you have been working with a Reference that only exists in the ```ReferenceModule```, but in a real game you will want your model to get backed by actual data. You can create a new component that extends the ```ReferenceProvider``` class in order to do this. At start up the ```ReferenceModule``` will grab this component and request the Reference from the provider. This Reference can be retrieved in whatever way makes sense for your game. The provided Reference will override any ```IDetails``` contained in the ```ReferenceModule's``` default Reference with its own. Effectively updating any ```DetailBinders``` to point to the new Reference. However, if there are any ```IDetails``` that exist in the ```ReferenceModule``` and not in the new Reference, then they will continue to exist as well. In this way, you can combine view only ```IDetails``` with backing models and access them from the same source instead of maintaining two sets of models or cramming view and data only stuff into a single object.

```ReferenceModules``` can have their Reference set via code by using the exposed Reference property at anytime during gameplay. ```ReferenceProviders``` simply provide a mechanism to assign the Reference at start up without having to access the ```ReferenceModule``` externally. A single example ```TemplateReferenceProvider``` has been added for testing purposes. It will provide the Reference contained in the assigned ```ReferenceTemplate``` and has a context menu item to set the Reference value for testing changing the Reference at different times during execution.

![provider_example](https://user-images.githubusercontent.com/7310389/134281237-c3f8dde4-c69d-4be9-9b9a-c154a92f03e7.gif)

#### Reference Templates
```ReferenceTemplates``` are Scriptable Objects that contain a Reference and behave almost identically to ```ReferenceModules```. These can be used to assign ```IDetails``` to ```ReferenceModules``` and as example models to stub into your game for testing. ```ReferenceModules``` have an Import button that can be used to import the ```IDetails``` from an existing ```ReferenceTemplate```. This will only add ```IDetails``` that didn't originally existing the ```ReferenceModule```. All other ```IDetails``` will be preserved. 

### States
#### State Module
#### State Binder
