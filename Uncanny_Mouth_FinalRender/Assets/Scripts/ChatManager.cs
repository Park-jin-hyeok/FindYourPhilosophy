using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using System.Linq;

public class ChatManager : MonoBehaviour
{
    public OnResponseEvent OnResponse;
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }
    public List<MouthAnimator> mouthControllers;
    public int nMemory = 20;

    private OpenAIApi openAI = new OpenAIApi();

    private List<ChatMessage> memory = new List<ChatMessage>();

    private ChatMessage mode; // header 
    private ChatMessage end_prompt; // tail 

    private ChatMessage summarize_prompt; 

    public GameObject PhilosopherHead;

    private WebSocketReceiver webSocketReceiver;
    private RunPython pythonActivator;
    private TextToSpeech textToSpeech;

    public bool doAskGPT = false;
    public string currentMsg = "";
    public bool doAssess = false;

    private string debug_dialogue;

    public List<ChatMessage> AddSystemMessage(ChatMessage newElement, ChatMessage endPrompt)
    {
        List<ChatMessage> modifiedMemory = new List<ChatMessage>(memory);

        modifiedMemory.Insert(0, newElement);
        modifiedMemory.Add(endPrompt);

        return modifiedMemory;
    }

    private void AddNewMessagetoMemory(ChatMessage message)
    {
        if (memory.Count < nMemory)
        {
            memory.Add(message);
        }
        else
        {
            memory.RemoveAt(0);
            memory.Add(message);
        }
    }

    public void ResetMemory()
    {
        memory.Clear();
    }

    public void AskChatGPT(string newText)
    {
        Debug.Log("Asking Uncanny: " + newText);
        StartCoroutine(AskChatGPTCoroutine(newText));
    }

    private IEnumerator AskChatGPTCoroutine(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        List<ChatMessage> reqMessage;

        newMessage.Content = newText;
        newMessage.Role = "user";

        AddNewMessagetoMemory(newMessage);
        reqMessage = AddSystemMessage(mode, end_prompt);

        //for (int i = 0; i < reqMessage.Count; i++)
        //{
        //    Debug.Log("Message " + i + ": ");
        //    Debug.Log("Role: " + reqMessage[i].Role);
        //    Debug.Log("Content: " + reqMessage[i].Content);
        //}

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = reqMessage;
        request.Model = "gpt-4o";
        request.MaxTokens = 500;
        Debug.Log("Sending Request: " + request);
        var responseTask = openAI.CreateChatCompletion(request);



       
        yield return new WaitUntil(() => responseTask.IsCompleted);

        var response = responseTask.Result;

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            AddNewMessagetoMemory(chatResponse);

            string responseText = chatResponse.Content;
            Debug.Log("Response: " + responseText);

            TextSpeech(responseText);
        }
    }


    public void EnableLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = true;
        }
    }

    public void DisableLipSync()
    {
        foreach (var controller in mouthControllers)
        {
            controller.lipSyncToggle = false;
        }
    }


    

    public IEnumerator AssessDialogueCoroutine()
    {
        // list of memory to string
        string dialogueHistory = string.Join("\n", memory.Select(msg => $"{msg.Role}: {msg.Content}"));

        string assessmentPromptContent = $"{summarize_prompt.Content}\n\nDialogue:\n{dialogueHistory}\n\n {summarize_prompt.Content}";

        ChatMessage assessmentPrompt = new ChatMessage
        {
            Content = assessmentPromptContent,
            Role = "system"
        };

        List<ChatMessage> reqMessage = new List<ChatMessage> { mode };
        reqMessage.AddRange(memory);
        reqMessage.Add(end_prompt);
        reqMessage.Add(assessmentPrompt);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = reqMessage,
            Model = "gpt-4o",
            MaxTokens = 500
        };

        Debug.Log("Sending Request for assessment");

        var responseTask = openAI.CreateChatCompletion(request);
        yield return new WaitUntil(() => responseTask.IsCompleted);

        var response = responseTask.Result;

        if (response.Choices != null && response.Choices.Count > 0)
        {
            string assessmentResponse = response.Choices[0].Message.Content;
            Debug.Log("Assessment Response: " + assessmentResponse);

            // 저장 및 출력!
            pythonActivator.saveAndPrintAssessment(assessmentResponse);
        }
    }

    public IEnumerator AssessDialogueCoroutine_debug()
    {
        string dialogueHistory = debug_dialogue;

        string assessmentPromptContent = $"{summarize_prompt.Content}\n\nDialogue:\n{dialogueHistory}";

        ChatMessage assessmentPrompt = new ChatMessage
        {
            Content = assessmentPromptContent,
            Role = "system"
        };

        List<ChatMessage> reqMessage = new List<ChatMessage> { mode };
        reqMessage.AddRange(memory);
        reqMessage.Add(end_prompt);
        reqMessage.Add(assessmentPrompt);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = reqMessage,
            Model = "gpt-4o",
            MaxTokens = 500
        };

        Debug.Log("Sending Request for assessment");

        var responseTask = openAI.CreateChatCompletion(request);
        yield return new WaitUntil(() => responseTask.IsCompleted);

        var response = responseTask.Result;

        if (response.Choices != null && response.Choices.Count > 0)
        {
            string assessmentResponse = response.Choices[0].Message.Content;
            Debug.Log("Assessment Response: " + assessmentResponse);

            // 저장 및 출력!
            pythonActivator.saveAndPrintAssessment(assessmentResponse);
        }
    }

    void Start()
    {
        webSocketReceiver = GetComponent<WebSocketReceiver>();

        if (webSocketReceiver == null)
        {
            Debug.LogError("WebsocketReciever not found on the same GameObject as Chatmanager.");
        }

        pythonActivator = GetComponent<RunPython>();

        if (pythonActivator == null)
        {
            Debug.LogError("RunPython not found on the same GameObject as Chatmanager.");
        }

        textToSpeech = GetComponent<TextToSpeech>();

        if (pythonActivator == null)
        {
            Debug.LogError("TextToSpeech not found on the same GameObject as Chatmanager.");
        }

        OnResponse.AddListener(TextSpeech);

        ChatMessage newMode = new ChatMessage(); // mode stating variable 
        // newMode.Content = "You are simulating a conversation where you, a knowledgeable philosopher, engage with individuals to discuss various philosophical topics. Your goal is to explore their ideas and thoughts deeply, asking insightful questions and providing thoughtful responses that encourage them to express their views on existence, reality, free will, ethics, knowledge, and the meaning of life. Through this dialogue, you will assess their philosophical inclinations.\r\n";
        newMode.Content = "You are simulating a conversation where you, a knowledgeable philosopher, engage with individuals to discuss various philosophical topics. " +
            "Your goal is to explore their ideas and thoughts deeply, asking insightful questions and providing thoughtful responses that encourage them to express their views on " +
            "existence, reality, free will, ethics, knowledge, and the meaning of life. Current Topic is: **Existence and Reality** **Free Will** **Ethics** **Knowledge and Epistemology ** **Meaning of Life**." +
            "Don't ask directly about how they think of each of the topics. Ask indirect questions which will reveal ones thoughts about the given topic. Don't keep asking similar questions, make it interesting and fun, You have to answer questions that people ask. \r\n" + 
            "it doesn't have to be about the above topics, tell interesting philosophical stories or ask interesting philosophical question. 한국어로 대답해야해";
        newMode.Role = "system";

        ChatMessage endPrompt = new ChatMessage();
        // endPrompt.Content = "Imagine a conversation between a philosopher and a modern-day individual, where the philosopher provides insightful answers that encourage deep thinking and reflection. Your goal is to draw out the individual's thoughts and provide thought-provoking responses that help them explore their ideas more deeply. You take the role of the philosopher. The user takes the role of the modern individual. Don't write anything like 'philosopher: there is no format, get to the talk right away. You should ask meaningful questions rather than given the answer. Generate as short response as possible.'\r\n";
        endPrompt.Content = "Imagine a conversation between a philosopher and a modern-day individual, where the philosopher provides insightful answers that encourage deep thinking and reflection. Your goal is to draw out the individual's thoughts and provide thought-provoking responses that help them explore their ideas more deeply. You take the role of the philosopher. The user takes the role of the modern individual. Don't write anything like 'philosopher: there is no format, get to the talk right away. " +
            "Current Topic is: **Existence and Reality** **Free Will** **Ethics** **Knowledge and Epistemology ** **Meaning of Life**.\" +\r\n            \"Don't ask directly about how they think of each of the topics. Ask indirect questions which will reveal ones thoughts about the given topic. Try your best to ask unasked topics." +
            "You should ask meaningful questions rather than given the answer. Generate as short response as possible.'\r\n";

        endPrompt.Role = "system";

        ChatMessage summaryEvokingPrompt = new ChatMessage();
        // summaryEvokingPrompt.Content = "Given the following dialogue, assess and score the individual's inclinations based on the following philosophical ideas:\r\n\r\n1. **Existence and Reality**\r\n2. **Free Will**\r\n3. **Ethics**\r\n4. **Knowledge and Epistemology**\r\n5. **Meaning of Life**\r\n\r\nDialogue:\r\n[Insert dialogue here]\r\n\r\nFor each topic, provide a score from 0 to 15 and a brief explanation of the score based on the individual's statements. Even the score doesn't seem to be evident, just give 7. The output should be formatted as follows.:\r\n\r\n### Uncanny Assessment\r\n\r\n**Topic 1: Existence and Reality (Skeptical 0-7, Positive 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 2: Free Will (Deterministic 0-7, Libertarian 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 3: Ethics (Moral Relativism 0-7, Moral Absolutism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 4: Knowledge and Epistemology (Empiricism 0-7, Rationalism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 5: Meaning of Life (Nihilism 0-7, Existentialism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]";
        summaryEvokingPrompt.Content = "Given the following dialogue, assess and score the individual's inclinations based on the following philosophical ideas:\r\n\r\n1. **Existence and Reality**\r\n2. **Free Will**\r\n3. **Ethics**\r\n4. **Knowledge and Epistemology**\r\n5. **Meaning of Life**\r\n\r\nDialogue:\r\n[Insert dialogue here]\r\n\r\nFor each topic, provide a score from 0 to 15 and a brief explanation of the score based on the individual's statements. Even the score doesn't seem to be evident, just give 7, always come up with some kind of explaination make it up. Be flexible and creative on how you assess the user, based not only on dialogue but meta dialogue. For example, if the user says nothing, write that he supports the importance of Sillence. Explaination should be 3 to 4 sentences. The output should be formatted as follows. Can you change only the Exploration part to Korean?:\r\n\r\n### Uncanny Assessment\r\n\r\n**Topic 1: Existence and Reality (Skeptical 0-7, Positive 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 2: Free Will (Deterministic 0-7, Libertarian 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 3: Ethics (Moral Relativism 0-7, Moral Absolutism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 4: Knowledge and Epistemology (Empiricism 0-7, Rationalism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]\r\n\r\n**Topic 5: Meaning of Life (Nihilism 0-7, Existentialism 8-15)**\r\n- Score: [0-15]\r\n- Explanation: [Explanation of the score]";
        summaryEvokingPrompt.Role = "system";

        debug_dialogue = "Philosopher: What are your thoughts on the nature of reality? Do you believe that what we perceive is the true nature of existence?\r\n\r\nUser: I've always wondered if what we see and experience is just a construct of our minds. It seems plausible that there could be more beyond our perception.\r\n\r\nPhilosopher: That's an interesting perspective. How do you feel about the concept of free will? Do you think our choices are truly our own, or are they determined by factors beyond our control?\r\n\r\nUser: I believe in free will to some extent, but it's hard to ignore the influence of our environment and upbringing on our decisions.\r\n\r\nPhilosopher: Indeed, the debate between determinism and free will is a complex one. Moving on to ethics, do you think moral values are absolute, or are they shaped by cultural and personal contexts?\r\n\r\nUser: I think moral values are mostly shaped by cultural and personal contexts. What is considered right or wrong can vary greatly between different societies and individuals.\r\n\r\nPhilosopher: That's a valid point. How about knowledge and epistemology? Do you believe that knowledge comes primarily from sensory experience, or is it derived from reason and logical thinking?\r\n\r\nUser: I believe that knowledge is a combination of both sensory experience and logical reasoning. We need our senses to gather information, but reasoning helps us make sense of it all.\r\n\r\nPhilosopher: A balanced view, indeed. Lastly, what are your thoughts on the meaning of life? Do you think there is an inherent purpose, or is it something we create for ourselves?\r\n\r\nUser: I lean towards the idea that we create our own meaning in life. It's up to each individual to find purpose and fulfillment in their own way.\r\n\r\n";

        mode = newMode;
        end_prompt = endPrompt;
        summarize_prompt = summaryEvokingPrompt;  
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            LogMemoryContents();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string a = "hi"; 
            AskChatGPT(a);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(AssessDialogueCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Assessment Debug");
            StartCoroutine(AssessDialogueCoroutine_debug());
        }

        // do run requests 
        if (doAskGPT)
        {
            doAskGPT = false;
            AskChatGPT(currentMsg);
        } 
        if (doAssess)
        {
            doAssess = false;
            StartCoroutine(AssessDialogueCoroutine());
        }
    }

    void TextSpeech(string responseText)
    {
        StartCoroutine(textToSpeech.ConvertAndPlayCoroutine(responseText));
    }

    //void TextSpeech(string responseText)
    //{
    //     TextToSpeech.Instance.ConvertAndPlay(responseText); 
    //}

    void LogMemoryContents()
    {
        Debug.Log("Current Memory Contents:");
        for (int i = 0; i < memory.Count; i++)
        {
            Debug.Log("Message " + i + ": ");
            Debug.Log("Role: " + memory[i].Role);
            Debug.Log("Content: " + memory[i].Content);
        }
    }
}
