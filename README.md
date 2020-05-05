# SIIE - Selectable Inversion Image Effect
![](https://img.shields.io/github/issues-closed-raw/QFSW/SIIE.svg?color=51c414) ![](https://img.shields.io/github/issues-raw/QFSW/SIIE.svg?color=c41414&style=popout)

This image effect allows you to effortlessly create selectively inverted parts of the screen

![](http://media.indiedb.com/images/members/5/4201/4200312/profile/Screenshot_2018-01-15_21.27.18.png)

### Getting Started
1. Creating the layer `SelectableInversion`
2. Add the `SelectableInversion` component to your main camera

![](http://media.indiedb.com/images/members/5/4201/4200312/profile/Capture.PNG)

### Options
 - **Use Colored Inversion** causes the image to approach a user specified color as the inversion approaches 50%, instead of the gray that would otherwise be achieved by combining an inverted image with a non inverted image
 - **Use Mask Color** will use the color of the mask at that pixel for the mid inversion color instead of a constant user defined color
 - **Clear Color** controls which color the Inversion Camera will clear to; you can also think of this as the default inversion value

_The script is contained under the `QFSW.SIIE` namespace_

### Installation via Package Manager

#### 2019.3+
Starting with Unity 2019.3, the package manager UI has support for git packages

Click the `+` to add a new git package and add `https://github.com/QFSW/SIIE.git` as the source

#### 2018.3 - 2019.2
To install via package manager, add the file `Packages/manifest.json` and add the following line to the `"dependencies"`
```
"com.qfsw.siie": "https://github.com/QFSW/SIIE.git"
```
Your file should end up like this 
```
{
  "dependencies": {
    "com.qfsw.siie": "https://github.com/QFSW/SIIE.git",
    ...
  },
}
```