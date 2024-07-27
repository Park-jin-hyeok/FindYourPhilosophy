import cv2
import numpy as np



class Camera:
    def __init__(self, camera_id, im_width, im_height):
        self.camera_id = camera_id
        self.cap = cv2.VideoCapture(camera_id)
        if not self.cap.isOpened():
            raise ValueError(f"Camera {camera_id} could not be opened.")
        
        # Set camera resolution
        self.cap.set(cv2.CAP_PROP_FRAME_WIDTH, im_width)
        self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT, im_height)
        
        print(f"Webcam {camera_id} opened successfully with resolution {im_width}x{im_height}. Press 'q' to quit.")

    def capture(self):
        ret, frame = self.cap.read()
        if not ret:
            raise ValueError(f"Failed to capture image from camera {self.camera_id}.")
        return frame

    def release(self):
        self.cap.release()

def nothing(x):
    pass

# Create a window
cv2.namedWindow('image')

# Create trackbars for color change
cv2.createTrackbar('HMin', 'image', 0, 179, nothing)  # Hue is from 0-179 for OpenCV
cv2.createTrackbar('SMin', 'image', 0, 255, nothing)
cv2.createTrackbar('VMin', 'image', 0, 255, nothing)
cv2.createTrackbar('HMax', 'image', 0, 179, nothing)
cv2.createTrackbar('SMax', 'image', 0, 255, nothing)
cv2.createTrackbar('VMax', 'image', 0, 255, nothing)

# Set default value for Max HSV trackbars
cv2.setTrackbarPos('HMax', 'image', 179)
cv2.setTrackbarPos('SMax', 'image', 255)
cv2.setTrackbarPos('VMax', 'image', 255)

screen_w = 1920
screen_h = 1080

camera1 = Camera(0, screen_w, screen_h)

while True:
    frame = camera1.capture()
    
    # Convert to HSV format and color threshold
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    
    # Get current positions of all trackbars
    hMin = cv2.getTrackbarPos('HMin', 'image')
    sMin = cv2.getTrackbarPos('SMin', 'image')
    vMin = cv2.getTrackbarPos('VMin', 'image')
    hMax = cv2.getTrackbarPos('HMax', 'image')
    sMax = cv2.getTrackbarPos('SMax', 'image')
    vMax = cv2.getTrackbarPos('VMax', 'image')

    # Determine if a dual filter is needed based on hMin and hMax
    if hMin < hMax:
        dual_filter = False
        lower_color = np.array([hMin, sMin, vMin])
        upper_color = np.array([hMax, sMax, vMax])
        mask = cv2.inRange(hsv, lower_color, upper_color)
    else:
        dual_filter = True
        lower_color_1 = np.array([0, sMin, vMin])
        upper_color_1 = np.array([hMax, sMax, vMax])
        lower_color_2 = np.array([hMin, sMin, vMin])
        upper_color_2 = np.array([179, sMax, vMax])
        
        mask1 = cv2.inRange(hsv, lower_color_1, upper_color_1)
        mask2 = cv2.inRange(hsv, lower_color_2, upper_color_2)
        mask = cv2.bitwise_or(mask1, mask2)

    result = cv2.bitwise_and(frame, frame, mask=mask)
    
    # Display the original and masked image
    cv2.imshow('image', result)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        print(f'hMin = {hMin} \nsMin = {sMin} \nvMin = {vMin} \nhMax = {hMax} \nsMax = {sMax} \nvMax = {vMax}')
        break

# Release the capture and close windows
camera1.release()
cv2.destroyAllWindows()
