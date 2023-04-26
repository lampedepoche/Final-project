# Final-project
CS316: Advance Programming Techniques

## Goal and features
The program allows you to draw rectangles and ellipses at the desired size with the possibility of customizing the colors of the border and the filling of the shape.
Its features are extremely basic: save as, open a new file, undo the last added shape, delete one shape at a time or all and export to SVG file.

## Acknowledgment
Two bookshelves were downloaded but the majority of the project was inspired by the work of Chritopher Diggins in the course. ChatGPT and StakOverflow have been very useful for saving, exporting to SVF files and solving bugs of all kinds.

## Screenshots
![image](https://user-images.githubusercontent.com/97594278/234481233-a53f6036-832a-4fb4-b1e4-832980eb3041.png)

Drawings of rectangles


![image](https://user-images.githubusercontent.com/97594278/234481713-f5f5c9bc-8f66-4e92-ba4b-acf88dc951e7.png)

Drawings of ellipses


![image](https://user-images.githubusercontent.com/97594278/234481282-e560e848-ef5a-4642-948b-ebc43d95d73d.png)

Selection/deleting of shapes (here, an ellipse)


## List of bugs and known issues
The selection function was originally intended to select a shape and allow the user to modify it afterwards (enlarge/shrink, change the background colour, or the border). Eventually, the selection function was turned into a delete function only. If there are several shapes in the same place, only the last one created will be deleted. It is also worth noting that the rectangle that is displayed to indicate a selected shape is not displayed in the foreground. In more complex designs with several shapes, the fact that the rectangle is displayed in the background makes this function unnecessary.
The undo function deletes added figures but is not able to recreate a recently deleted figure.

## To-Do
A lot.
Mainly basic functions like Save, New, Redo and a proper Undo and Select would be needed. Ideas for slightly advanced functions lived in the imagination of the programmer: possibility to draw stars, to make fractals by pressing the arrows on the keyboard, resize a shape, etc.
