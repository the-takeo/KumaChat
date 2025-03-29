using KumaAI;

Console.WriteLine("クマだが?");

var kuma= new Kuma();

while (true)
{
    var query = Tuning.RolePlayKumaQuery + Console.ReadLine();
    Console.WriteLine(await kuma.GetKumaMessage(query));
}