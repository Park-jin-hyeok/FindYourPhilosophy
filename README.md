# Uncanny: Find My Philosophy

Uncanny: Find My Philosophy는 현대 사회에서 철학적 사고를 장려하기 위해 개발된 프로젝트입니다. 사용자는 철학적 질문에 답변함으로써 자신의 철학적 성향을 탐구하고, 현실감 있는 철학자와의 대화를 통해 지속적인 철학적 사고를 이끌어갈 수 있습니다.

## 프로젝트 구성

uncanny-find-my-philosophy/
├── CapstoneRenew/ # 유니티 프로젝트 - 눈 화면 파일
│ ├── Assets/
│ ├── ProjectSettings/
│ └── ...
├── Uncanny_Mouth_FinalRender/ # 유니티 프로젝트 - 입 화면 파일
│ ├── Assets/
│ ├── ProjectSettings/
│ └── ...
├── Uncanny_off-axis-Binocular-Position-Retreival/ # 파이썬 프로젝트 - 카메라를 통해 HSV 검출 파일
│ ├── camera.py
│ ├── utils.py
│ ├── main.py
│ └── ...
├── Uncanny_off-axis-projection-unity-master/ # 유니티 프로젝트 - 배경 파일
│ ├── Assets/
│ ├── ProjectSettings/
│ └── ...
├── Uncanny_print_folder/ # 파이썬 프로젝트 - 프린트 파일과 sever.js 서버파일
│ ├── server.js
│ ├── print_result.py
│ ├── message_parse.py
│ ├── ...
├── README.md # 프로젝트 설명 파일
└── .gitignore # Git에 포함되지 않을 파일 및 디렉토리 목록

## 프로그램 사용법
(서버 cmd 켜서 port 주소 확인 하는것 넣기)
(확인 후 노트북에서 키는 파일에 주소에 넣기)
(프린트 py 저장할 공간 알려주기)
1. 서버 실행
server.js 파일을 실행합니다. (추후 업데이트 예정)

2. 카메라 위치 감지 실행
Uncanny_off-axis-Binocular-Position-Retreival 디렉토리에서 bnc_v1.0__main_udp.py 파일을 실행합니다.

카메라 2대가 연결되어 있어야 합니다.

스크립트 내의 설정을 다음과 같이 변경합니다:

python
코드 복사
do_plot = True
주석을 제거하여 플롯을 활성화합니다:

python
코드 복사
##plt.ioff()
##plt.show()
색상을 조절할 수 있습니다:

python
코드 복사
red = (190, 108, 112)
blue = (31, 117, 210)
yellow = (194, 211, 84)
green = (45, 127, 79)
purple = (159, 147, 245)
3. 배경 조정 및 프로젝션 설정
Uncanny_off-axis-projection-unity-master 디렉토리에서 유니티 프로젝트를 실행하고, 알맞은 씬을 선택합니다.

4. 눈과 입 애니메이션 설정
다음 유니티 프로젝트를 실행합니다:

Uncanny_Mouth_FinalRender
CapstoneRenew
각 프로젝트에서 알맞은 씬을 선택하고 실행합니다.
