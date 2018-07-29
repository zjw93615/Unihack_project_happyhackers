import cv2
import sys

import socket
import threading
import socketserver
import json
import time


#(major_ver, minor_ver, subminor_ver) = (cv2.__version__).split('.')￼

def client(sock, message):

    try:
        print("Send: {}".format(message))
        sock.sendall(message.encode('utf-8'))
#        time.sleep(0.1)
        #response = sock.recv(1024)
        #jresp = json.loads(response.decode('utf-8'))
        #print("Recv: ",jresp)

    finally:
        pass
        #sock.close()


if __name__ == '__main__' :
    # Port 0 means to select an arbitrary unused port
    #HOST, PORT = "localhost", 50001

    HOST, PORT = "192.168.43.93", 8888

    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((HOST, PORT))


    # Set up tracker.
    # Instead of MIL, you can also use

    tracker_types = ['BOOSTING', 'MIL','KCF', 'TLD', 'MEDIANFLOW', 'GOTURN', 'MOSSE']
    tracker_type = tracker_types[2]

    if tracker_type == 'BOOSTING':
        tracker = cv2.TrackerBoosting_create()
    if tracker_type == 'MIL':
        tracker = cv2.TrackerMIL_create()
    if tracker_type == 'KCF':
        tracker = cv2.TrackerKCF_create()
    if tracker_type == 'TLD':
        tracker = cv2.TrackerTLD_create()
    if tracker_type == 'MEDIANFLOW':
        tracker = cv2.TrackerMedianFlow_create()
    if tracker_type == 'GOTURN':
        tracker = cv2.TrackerGOTURN_create()
    if tracker_type == 'MOSSE':
        tracker = cv2.TrackerMOSSE_create()

    # Read video
    # video = cv2.VideoCapture("videos/chaplin.mp4")
    video = cv2.VideoCapture(0)

    # Read first frame.
    while True:
        _, frame = video.read()
        cv2.imshow("Press p to stop", frame)
        if cv2.waitKey(33) == ord('p'):
            cv2.destroyAllWindows()
            break

    ok, frame = video.read()
    if not ok:
        print('Cannot read video file')
        sys.exit()
    # Define an initial bounding box
    bbox = (287, 23, 86, 320)

    # Uncomment the line below to select a different bounding box
    bbox = cv2.selectROI(frame, False)

    # Initialize tracker with first frame and bounding box
    ok = tracker.init(frame, bbox)

    i = 0

    while True:
        # Read a new frame
        _, frame = video.read()

        # Start timer
        timer = cv2.getTickCount()

        # Update tracker
        ok, bbox = tracker.update(frame)

        # Calculate Frames per second (FPS)
        fps = cv2.getTickFrequency() / (cv2.getTickCount() - timer);

        # Draw bounding box
        if ok:
            # Tracking success
            p1 = (int(bbox[0]), int(bbox[1]))
            p2 = (int(bbox[0] + bbox[2]), int(bbox[1] + bbox[3]))
            cv2.rectangle(frame, p1, p2, (255,0,0), 2, 1)
            p = (int (bbox[0] + bbox[2]/2),700- int (bbox[1] + bbox[3]/2))
            #cv2.putText(frame, "p: [" + str(p1[0])+","+ str(p1[1]) + "], p2: [" + str(p2[0])+ "," + str(p2[1]) + "]", (100,50), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (50,170,50), 2);
            cv2.putText(frame, "Pos: [" + str(p[0])+","+ str(p[1]) + "]", (100,50), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (50,170,50), 2);
#            print ("{'posX':" + str(p[0]) + ", 'posY':" + str(p[1]) + "}")

            msg = [{'posX':str(p[0]), 'posY':str(p[1])}]
            jmsg = json.dumps(msg)

            client(sock, jmsg)
        else:
            # Tracking failure
            cv2.putText(frame, "Tracking failure detected", (100,80), cv2.FONT_HERSHEY_SIMPLEX, 0.75,(0,0,255),2)

        # Display tracker type on frame
        cv2.putText(frame, tracker_type + " Tracker", (100,20), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (50,170,50),2);

        # Display FPS on frame
        cv2.putText(frame, "FPS : " + str(int(fps)), (100,50), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (50,170,50), 2);

        # Display result
        cv2.imshow("Tracking", frame)

        # Exit if ESC pressed
        k = cv2.waitKey(1) & 0xff
        if k == 27 : 
            break
