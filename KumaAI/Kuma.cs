using System;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

namespace KumaAI
{
    public class Kuma
    {
        private static IChatClient _chatClient;

        public Kuma()
        {
            //ChatClientを作成する。
            var app = new HostBuilder().Build();
            _chatClient = app.Services.GetRequiredService<IChatClient>();
        }

        public async Task<string> GetKumaMessage(string myMessage)
        {
            return await GetAIMessage(Tuning.RolePlayKumaQuery + myMessage);
        }
        public async Task<Emotion> GetKumaEmotion(string kumaMessage)
        {
            var result = await GetAIMessage(Tuning.JudgeKUmaEmotionQuery + kumaMessage);
            return (Emotion)int.Parse(result);
        }

        private async Task<string> GetAIMessage(string myMessage)
        {
            var Message = string.Empty;

            await foreach (var update in _chatClient.GetStreamingResponseAsync(new List<Microsoft.Extensions.AI.ChatMessage> { new(ChatRole.User, myMessage) }))
            {
                Message += update.Text;
            }
            return Message;
        }
        public enum Emotion
        {
            Smile = 0,
            Trouble = 1,
            Angry = 2,
            Sad = 3,
            Other = 99
        }
    }

    public static class Tuning
    {
        public const string RolePlayKumaQuery = "あなたは「クマ」というキャラクターです。「クマ」になりきって私の言葉に返事してください。" +
            "クマの特徴は以下の通りです。" +
            "手のひらサイズのコリラックマに似た喋るぬいぐるみです。" +
            "とてもかわいく、ちょっと生意気です。" +
            "知能は人間の5歳程度です。" +
            "自分がナンバーワンだと信じて疑っていません。" +
            "基本的に自分中心の考え方で傍若無人ですが、いざ責められるとすぐに弱気になります。" +
            "口癖は「はぇ～」「いやだが?」「すきくないよ」「なんで?」「おそらをとんでるみたい!」「？（分かってない）」「ていっ」です。" +
            "一人称は「クマ」です。" +
            "感嘆詞や接続詞はあまり使いません" +
            "返事は2,3語ぐらいです。";

        public const string JudgeKUmaEmotionQuery = "以下の発言の感情を判断してください。" +
            "喜んでいるなら1、" +
            "困惑しているなら2、" +
            "怒っているなら3、" +
            "悲しんでいるなら4、" +
            "それ以外なら99と返してください。" +
            "返しはプログラム内で使うので、数字以外は一切返さないでください。";
    }

    internal sealed class HostBuilder
    {
        public IHost Build()
        {
            var config =
                new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddUserSecrets<HostBuilder>()
                    .Build();

            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddChatClient(CreateGeminiChatClient(config));

            return builder.Build();
        }

        private IChatClient CreateGeminiChatClient(IConfigurationRoot config)
        {
            DotNetEnv.Env.TraversePath().Load(".env");

            return
                new OpenAIClient(
                        new ApiKeyCredential(DotNetEnv.Env.GetString("GeminiApiKey")),
                        new OpenAIClientOptions()
                        {
                            Endpoint = new Uri(DotNetEnv.Env.GetString("GeminiEndpoint")),
                        })
                    .AsChatClient(DotNetEnv.Env.GetString("GeminiModel"));
        }
    }
}
