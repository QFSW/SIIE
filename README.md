# SIIE - Selectable Inversion Image Effect
![](https://img.shields.io/github/issues-closed-raw/QFSW/SIIE.svg?color=51c414) ![](https://img.shields.io/github/issues-raw/QFSW/SIIE.svg?color=c41414&style=popout)

This image effect allows you to effortlessly create selectively inverted parts of the screen

![](http://media.indiedb.com/images/members/5/4201/4200312/profile/Screenshot_2018-01-15_21.27.18.png)

Begin by creating the layer "SelectableInversion", then adding the image effect to your main camera.

![](http://media.indiedb.com/images/members/5/4201/4200312/profile/Capture.PNG)

If you select Use Colored Inversion, then the image will approach a user specified color as the inversion approaches 50% instead of the gray that would otherwise be achieved by combining an inverted image with a non inverted image.

Clicking Use Mask Color will use the color of the mask at that pixel for the mid inversion color instead of a constant user defined color

The Clear Color lets you chose what color the Inversion Camera will clear to, you can also think of this as the default inversion value

To manipulate the effect via code, ensure to add `using QFSW.SIIE;` to your script

# Donate
If you enjoyed this product and would like to see more, please consider donating or purchasing some of our other products.
 - [Unity Asset Store Products](https://assetstore.unity.com/publishers/18921)
 - [Steam Games](https://store.steampowered.com/developer/QFSW)
 - [Patreon](https://www.patreon.com/QFSW)
 - [PayPal](https://www.paypal.me/qfsw)
