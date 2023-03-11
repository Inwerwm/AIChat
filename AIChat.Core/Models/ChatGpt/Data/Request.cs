using System.Text.Json.Serialization;

namespace AIChat.Core.Models.ChatGpt.Data;

internal class RequestBody
{
    /// <summary>
    /// 使用するモデルのID。
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model
    {
        get; init;
    }

    /// <summary>
    /// <see href="https://platform.openai.com/docs/guides/chat/introduction">チャット形式</see>で、チャット補完を生成するためのメッセージ。
    /// </summary>
    [JsonPropertyName("messages")]
    public required List<Message> Messages
    {
        get; init;
    }

    /// <summary>
    /// サンプリング温度は0～2の間で指定します。0.8のような高い値は出力をよりランダムにし、0.2のような低い値は出力をより集中させ決定論的にします。一般的には、この値か top_p のどちらかを変更することをお勧めしますが、両方を変更することはできません。
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature
    {
        get; init;
    }

    /// <summary>
    /// 温度によるサンプリングに代わるものとして、核サンプリングと呼ばれる、確率質量がtop_pのトークンの結果を考慮するモデルがある。つまり、0.1は、上位10%の確率質量からなるトークンだけを考慮することを意味します。私たちは一般的に、これか温度を変更することを推奨しますが、両方を変更することはできません。
    /// </summary>
    [JsonPropertyName("top_p")]
    public double? TopP
    {
        get; init;
    }

    /// <summary> 
    /// 各入力メッセージに対して、チャット補完の選択肢をいくつ生成するか。
    /// </summary> 
    [JsonPropertyName("n")]
    public int? N
    {
        get; init;
    }

    /// <summary> 
    /// 設定されている場合、ChatGPTのように部分的なメッセージの差分が送信されます。トークンはデータのみの<see href="https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#Event_stream_format">サーバー送信イベント</see>として、利用可能になると送信され、[DONE]というメッセージのデータでストリームは終了します。
    /// </summary> 
    [JsonPropertyName("stream")]
    public bool? Stream
    {
        get; init;
    }

    /// <summary> 
    /// APIがさらなるトークンの生成を停止するシーケンスを最大4つまで指定できます。
    /// </summary> 
    [JsonPropertyName("stop")]
    public List<string>? Stop
    {
        get; init;
    }

    /// <summary>
    /// 生成される答えに許可されるトークンの最大数です。デフォルトでは、モデルが返すことのできるトークン数は（4096 - プロンプトトークン数）となります。
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens
    {
        get; init;
    }

    /// <summary>
    /// 2.0から2.0の間の数値。正の値は、新しいトークンがこれまでのテキストに現れたかどうかに基づいてペナルティを与え、モデルが新しいトピックについて話す可能性を高める。
    /// <para><see href="https://platform.openai.com/docs/api-reference/parameter-details">頻度や出現ペナルティについては、こちらをご覧ください。</see></para>
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty
    {
        get; init;
    }

    /// <summary>
    /// 2.0から2.0の間の数値。正の値は、これまでのテキストにおける既存の頻度に基づいて新しいトークンにペナルティを与え、モデルが同じ行をそのまま繰り返す可能性を減少させる。
    /// <para><see href="https://platform.openai.com/docs/api-reference/parameter-details">頻度や出現ペナルティについては、こちらをご覧ください。</see></para>
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty
    {
        get; init;
    }

    /// <summary>
    /// 指定したトークンが補完に現れる可能性を変更する。
    /// <para>トークン（トークナイザーのトークンIDで指定）を-100から100までのバイアス値にマッピングするjsonオブジェクトを受け取ります。
    /// 数学的には、バイアスはサンプリング前にモデルによって生成されたロジットに加算されます。
    /// 正確な効果はモデルによって異なりますが、-1～1の値は、選択の可能性を減少または増加させるはずです。</para>
    /// </summary>
    [JsonPropertyName("logit_bias")]
    public Dictionary<string, double>? LogitBias
    {
        get; init;
    }

    /// <summary>
    /// エンドユーザーを表す一意の識別子で、OpenAIの監視や不正利用の検出に役立ちます。 <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [JsonPropertyName("user")]
    public string? User
    {
        get; init;
    }
}