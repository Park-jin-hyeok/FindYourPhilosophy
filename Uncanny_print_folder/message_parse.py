import re

run_exp = True
exp_txt = "Assessment Response: ### Uncanny Assessment\
    \
    **Topic 1: Reality (Skeptical 0-7, Positive 8-15)**\
    - Score: 10\
    - Explanation: The user expresses a healthy skepticism about the nature of reality and suggests that what we perceive might not be the full extent of existence. This indicates a balanced view that leans slightly towards skepticism but remains open to deeper truths.\
    \
    **Topic 2: Free Will (Deterministic 0-7, Libertarian 8-15)**\
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
    **Topic 5: Meaning of Life (Nihilism 0-7, Existentialism 8-15)**\
    - Score: 11\
    - Explanation: The user leans towards existentialism, believing that individuals create their own meaning and purpose in life. This reflects a proactive and self-determined approach to finding purpose.\
    " 

# # prompt
# def parse_msg(txt) :
#     # complete this code 
#     # result should be 

#     #(this is a example)
#     topic_name_list = ["Existence and Reality", ...]
#     topic_name2both_extreme_name = {
#         "Existence and Reality": ['Skeptical', 'Positive'],
#         ...
#     }

#     topic_name2score = {
#         "Existence and Reality": 10, 
#         ...
#     }

#     topic_name2explanation = {
#         "Existence and Reality": ['The user expresses a healthy skepticism about the nature of reality and suggests that what we perceive might not be the full extent of existence. This indicates a balanced view that leans slightly towards skepticism but remains open to deeper truths.']
#     }

#     return (topic_name_list, topic_name2both_extreme_name, topic_name2explanation)

def parse_msg(txt):
    # Split the input text into individual topics
    topics = re.split(r'\*\*Topic \d+:', txt)[1:]

    topic_name_list = []
    topic_name2both_extreme_name = {}
    topic_name2score = {}
    topic_name2explanation = {}

    for topic in topics:
        # Extract the topic name
        topic_name_match = re.search(r'(.*?)( \(.*?\))', topic)
        topic_name = topic_name_match.group(1).strip()
        topic_name_list.append(topic_name)
        
        # Extract the extremes
        extremes = re.search(r'\((.*?)\)', topic).group(1).split(',')
        extremes = [extreme.strip() for extreme in extremes]
        
        # Extract the score
        score = int(re.search(r'\- Score: (\d+)', topic).group(1).strip())
        
        # Extract the explanation
        explanation = re.search(r'\- Explanation: (.*?)(?=(\*\*|$))', topic, re.DOTALL).group(1).strip()
        
        # Save the data in dictionaries
        topic_name2both_extreme_name[topic_name] = [extreme.split()[0] for extreme in extremes]
        topic_name2score[topic_name] = score
        topic_name2explanation[topic_name] = explanation

    return (topic_name_list, topic_name2both_extreme_name, topic_name2score, topic_name2explanation)


def main(): 
    if run_exp: 
        res = (parse_msg(exp_txt))

        print(res)
    else:
        pass

if __name__=="__main__":
    main()