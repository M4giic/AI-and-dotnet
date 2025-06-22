// See https://aka.ms/new-console-template for more information

using AutoGen;
using AutoGen.Core;
using FluentAssertions;

Console.WriteLine("Hello, World!");

string apiKey = "add your OpenAI API key here"; 
string modelId = "gpt-4";

var teacher = new AssistantAgent(
    name: "teacher",
    systemMessage: @"You are a teacher that create pre-school math question for student and check answer.
If the answer is correct, you terminate conversation by saying [TERMINATE].
If the answer is wrong, you ask student to fix it.",
    llmConfig: new ConversableAgentConfig
    {
        Temperature = 0,
        ConfigList = [new OpenAIConfig(apiKey, modelId) ]
            
    })
    .RegisterPostProcess(async (_, reply, _) =>
    {
        if (reply.GetContent()?.ToLower().Contains("terminate") is true)
        {
            return new TextMessage(Role.Assistant, GroupChatExtension.TERMINATE, from: reply.From);
        }

        return reply;
    })
    .RegisterPrintMessage();

var student = new AssistantAgent(
    name: "student",
    systemMessage: "You are a student that answer question from teacher",
    llmConfig: new ConversableAgentConfig
    {
        Temperature = 0,
        ConfigList = [new OpenAIConfig(apiKey, modelId)],
    })
    .RegisterPrintMessage();

// start the conversation
var conversation = await student.InitiateChatAsync(
    receiver: teacher,
    message: "Hey teacher, please create math question for me.",
    maxRound: 10);


conversation.Count().Should().BeLessThan(10);
conversation.Last().IsGroupChatTerminateMessage().Should().BeTrue();