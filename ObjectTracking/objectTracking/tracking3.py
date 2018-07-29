import cv2
import numpy as np

device = cv2.VideoCapture(0)

while True:
    _, img = device.read()
    hsv = cv2.cvtColor(img,cv2.COLOR_BGR2HSV)

# for red
    red_lower_range = np.array([136,87,111],np.uint8)
    red_upper_range = np.array([180,255,255],np.uint8)
# for blue
    blue_lower_range = np.array([99,115,150],np.uint8)
    blue_upper_range = np.array([110,255,255],np.uint8)

    red = cv2.inRange(hsv, red_lower_range, red_upper_range)
    blue = cv2.inRange(hsv, blue_lower_range, blue_upper_range)


    # Morphological transformation, Dilation
    kernal = np.ones((5,5),"uint8")

    red= cv2.dilate(red,kernal)
    res = cv2.bitwise_and(img, img, mask= red)

    blue= cv2.dilate(blue,kernal)
    res1 = cv2.bitwise_and(img, img, mask= blue)

    # Tracking the Red Color
    (_,contours,hierarchy) = cv2.findContours(red,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)

    for pic, contour in enumerate(contours):
        area = cv2.contourArea(contour)
        if (area > 300):
            x,y,w,h = cv2.boundingRect(contour)
            img = cv2.rectangle(img,(x,y),(x+w,y+h),(0,0,255),2)
            cv2.putText(img,"RED", (x,y),cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0,0,255))


    # Tracking the Blue Color
    (_,contours,hierarchy) = cv2.findContours(blue,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)

    for pic, contour in enumerate(contours):
        area = cv2.contourArea(contour)
        if (area > 300):
            x,y,w,h = cv2.boundingRect(contour)
            img = cv2.rectangle(img,(x,y),(x+w,y+h),(255,0,0),2)
            cv2.putText(img,"BLUE", (x,y),cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255,0,0))

    cv2.imshow("Result",img)


    if cv2.waitKey(1) == 27:
        break

device.release()
cv2.destroyAllWindows()
