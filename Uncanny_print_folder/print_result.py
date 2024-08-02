import matplotlib.pyplot as plt
import numpy as np
from fpdf import FPDF
import os
import win32print
import win32api
from PIL import Image

from message_parse import parse_msg

# 철학자 이진 점수 매핑 딕셔너리
philosophers = {
    '11111': 'Immanuel Kant',
    '11110': 'G.W.F. Hegel',
    '11101': 'René Descartes',
    '11100': 'Baruch Spinoza',
    '11011': 'Søren Kierkegaard',
    '11010': 'Jean-Paul Sartre',
    '11001': 'William James',
    '11000': 'David Hume',
    '10111': 'G. W. Leibniz',
    '10110': 'Thomas Hobbes',
    '10101': 'John Locke',
    '10100': 'George Berkeley',
    '10011': 'Friedrich Nietzsche',
    '10010': 'Arthur Schopenhauer',
    '10001': 'B.F. Skinner',
    '10000': 'Karl Marx',
    '01111': 'Plato',
    '01110': 'Aristotle',
    '01101': 'Thomas Aquinas',
    '01100': 'Augustine of Hippo',
    '01011': 'Albert Camus',
    '01010': 'Simone de Beauvoir',
    '01001': 'John Stuart Mill',
    '01000': 'J.L. Mackie',
    '00111': 'G.E. Moore',
    '00110': 'A.J. Ayer',
    '00101': 'W.V.O. Quine',
    '00100': 'Saul Kripke',
    '00011': 'Ludwig Wittgenstein',
    '00010': 'Michel Foucault',
    '00001': 'Richard Rorty',
    '00000': 'Jacques Derrida'
}

def make_image_transparent(input_path, output_path, transparency):
    img = Image.open(input_path).convert("RGBA")
    datas = img.getdata()
    
    new_data = []
    for item in datas:
        if item[3] > 0:  # alpha 값이 0보다 큰 경우
            new_data.append((item[0], item[1], item[2], int(item[3] * transparency)))
        else:
            new_data.append(item)
    
    img.putdata(new_data)
    img.save(output_path, "PNG")

def plot_pentagon_scores_adjusted(
        topic_name_list, 
        topic2extremes,
        topic2score, 
        topic2explanation, 
        pdf_path, 
        background_image_path,
        explanation_position):
    
    # 주제 리스트
    labels = topic_name_list

    # 점수 리스트
    scores = [topic2score[name] for name in labels]
    normalized_scores = [score / 15.0 for score in scores]  # 15로 나눈 값으로 정규화
    
    # 이진수 문자열로 변환
    binary_score = ''.join(map(str, [score // 8 for score in scores]))
    print(binary_score)
    # 철학자 이름 찾기
    philosopher = philosophers.get(binary_score, "Unknown Philosopher")
    
    # 철학자 이미지 경로 설정
    extra_image_path = f'C:\\Users\\CAU\\CapStone\\phil\\{binary_score}.png'

    # 글 리스트 
    explanations = [topic2explanation[name] for name in labels]
    del explanations[4]
    
    # 5개의 설명을 하나의 긴 문자열로 합치기
    combined_explanation = "\n\n".join(explanations)

    
    # 각 평가 항목에 대한 각도를 계산합니다.
    angles = np.linspace(0, 2 * np.pi, len(labels), endpoint=False).tolist()
    
    # 오각형을 닫기 위해 처음 점수를 다시 추가합니다.
    normalized_scores += normalized_scores[:1]
    angles += angles[:1]
    
    # 플롯을 초기화합니다.
    fig, ax = plt.subplots(figsize=(6, 6), subplot_kw=dict(polar=True))
    
    # 색상 설정
    line_color = '#343029'
    fill_color = '#343029'
    fill_alpha = 0.1  # 데이터 오각형의 투명도 조정

    # 데이터를 그리고 색상과 투명도를 설정합니다.
    ax.fill(angles, normalized_scores, color=fill_color, alpha=fill_alpha)
    ax.plot(angles, normalized_scores, color=line_color, linewidth=2)
    
    # 기본 오각형 그리기
    ax.set_rscale('linear')
    ax.set_ylim(0, 1)
    ax.set_yticks([0.2, 0.4, 0.6, 0.8, 1.0])
    ax.yaxis.set_tick_params(labelsize=0)  # y축 눈금 제거
    ax.grid(color='gray', linestyle='-', linewidth=0.5)  # 그리드 투명도 조정

    # 라벨을 설정합니다.
    ax.set_yticklabels([], fontsize=13)
    ax.set_xticks(angles[:-1])
    ax.set_xticklabels(["Reality", "Free", "Ethics", "Knowledge", "Meaning"], fontsize=20)

    # 그래프를 이미지 파일로 저장
    image_path = "C:\\Users\\CAU\\CapStone\\pentagon_score_chart.png"
    plt.savefig(image_path, transparent=True)
    plt.close(fig)
    
    # PDF 생성
    pdf = FPDF(orientation='L')  # 페이지 방향을 가로로 설정
    pdf.add_page()
    
    # 배경 이미지 추가
    pdf.image(background_image_path, x=0, y=0, w=297, h=210)
    
    # 유니코드 폰트 추가
    pdf.add_font('Malgun Gothic', '', 'C:/Windows/Fonts/malgun.ttf', uni=True)
    pdf.set_font('Malgun Gothic', '', 10)
    
    # 그래프 이미지를 PDF에 추가
    pdf.image(image_path, x=217, y=0, w=70, h=70)
    
    # 추가 이미지도 PDF에 추가
    transparent_image_path = 'C:\\Users\\CAU\\CapStone\\transparent_image.png'
    make_image_transparent(extra_image_path, transparent_image_path, 0.4) 
    pdf.image(transparent_image_path, x=149, y=0, w=71, h=71)
    
    pdf.set_right_margin(25)
    
    # 철학자 이름 추가
    pdf.set_xy(155, 55)
    pdf.set_font_size(17)
    pdf.multi_cell(0, 10, f"{philosopher}\n")
    
    # 긴 설명 문자열 추가
    pdf.set_xy(explanation_position[0], explanation_position[1])
    pdf.set_font_size(10)
    pdf.multi_cell(110, 5, combined_explanation)  # 너비를 110으로 설정
    
    # PDF 저장
    pdf.output(pdf_path)
    
    # 인쇄 명령 실행 (Windows의 경우)
    printer_name = win32print.GetDefaultPrinter()
    win32api.ShellExecute(
        0,
        "print",
        pdf_path,
        f'/d:"{printer_name}"',
        ".",
        0
    ) 

    return 0


pdf_path = 'C:\\Users\\CAU\\CapStone\\pentagon_score_chart.pdf'
background_image_path = 'C:\\Users\\CAU\\CapStone\\background.png' 

assessment_path = "C:/Users/CAU/Capstone/temp.txt"

with open(assessment_path, "r", encoding="utf-8") as file:
    message = file.read()

exp_txt = "Assessment Response: ### Uncanny Assessment\
    \
    **Topic 1: Reality (Skeptical 0-7, Positive 8-15)**\
    - Score: 10\
    - Explanation: The user expresses a healthy skepticism about the nature of reality and suggests that what we perceive might not be the full extent of existence. This indicates a balanced view that leans slightly towards skepticism but remains open to deeper truths.\
    \
    **Topic 2: Free (Deterministic 0-7, Libertarian 8-15)**\
    - Score: 7\
    - Explanation: The user believes in free will to some extent but acknowledges the significant influence of environmental factors and upbringing. This reflects a nuanced position that leans slightly towards determinism.\
    \
    **Topic 3: Ethics (Moral Relativism 0-7, Moral Absolutism 8-15)**\
    - Score: 6\
    - Explanation: The user thinks that moral values are shaped by cultural and personal contexts, indicating a relativistic view of ethics where morality is seen as flexible and context-dependent.\
    \
    **Topic 4: Knowledge (Empiricism 0-7, Rationalism 8-15)**\
    - Score: 10\
    - Explanation: The user believes that knowledge arises from both sensory experience and logical reasoning, suggesting a balanced approach that integrates elements of both empiricism and rationalism.\
    \
    **Topic 5: Meaning (Nihilism 0-7, Existentialism 8-15)**\
    - Score: 6\
    - Explanation: The user leans towards existentialism, believing that individuals create their own meaning and purpose in life. This reflects a proactive and self-determined approach to finding purpose.\
    " 

if message is None:
    message = exp_txt

# 웹소켓에서 메시지가 올 시 파싱합니다 
# topic_name_list, topic_name2both_extreme_name, topic_name2explanation
name_ls, name2both_extreme_name, name2score, name2explanation = parse_msg(message) 

# 설명 위치 지정 (x, y 좌표)
explanation_position = (162, 86)  # 설명이 들어갈 시작 위치

is_done = plot_pentagon_scores_adjusted(
                topic_name_list=name_ls,
                topic2extremes=name2both_extreme_name,
                topic2score=name2score,
                topic2explanation=name2explanation,
                pdf_path=pdf_path,
                background_image_path=background_image_path,
                explanation_position=explanation_position
            )

if is_done:
    # no problem 
    pass 
else: 
    pass  
