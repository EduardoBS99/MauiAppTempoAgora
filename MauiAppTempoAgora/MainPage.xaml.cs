using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Threading.Tasks;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        // Método auxiliar para traduzir/clasificar o clima
        private string TraduzirClima(string climaIngles)
        {
            return climaIngles switch
            {
                "Clear" => "Ensolarado",
                "Clouds" => "Nublado",
                "Rain" => "Chuvoso",
                "Snow" => "Nevando",
                "Drizzle" => "Chuviscando",
                "Thunderstorm" => "Tempestade",
                "Mist" => "Neblina",
                "Haze" => "Nevoeiro",
                "Fog" => "Nevoeiro",
                _ => climaIngles // se não encontrar, mostra o original
            };
        }

        // Método auxiliar para interpretar visibilidade em metros
        private string InterpretarVisibilidade(int? visibilidade)
        {
            if (visibilidade == null) return "Sem informação";

            if (visibilidade >= 10000)
                return "Excelente (acima de 10 km)";
            else if (visibilidade >= 5000)
                return "Boa (5 a 10 km)";
            else if (visibilidade >= 1000)
                return "Moderada (1 a 5 km)";
            else
                return "Baixa (menos de 1 km)";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";
                        dados_previsao =
                            $"Latitude: {t.lat} \n" +
                            $"Longitude: {t.lon} \n" +
                            $"Nascer do Sol: {t.sunrise} \n" +
                            $"Por do Sol: {t.sunset} \n" +
                            $"Temperatura Max: {t.temp_max}°C \n" +
                            $"Temperatura Min: {t.temp_min}°C \n" +
                            $"Visibilidade: {InterpretarVisibilidade(t.visibility)} \n" +
                            $"Velocidade do vento: {t.speed} m/s \n\n" +
                            $"O clima se apresenta {TraduzirClima(t.main)}.";

                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        // Caso a cidade não seja encontrada
                        await DisplayAlert("Cidade não encontrada",
                                           "Não conseguimos localizar a cidade informada. Verifique o nome digitado.",
                                           "OK");
                        lbl_res.Text = "";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }
            }
            catch (HttpRequestException)
            {
                // Caso não haja conexão com a internet
                await DisplayAlert("Sem conexão",
                                   "Verifique sua conexão com a internet e tente novamente.",
                                   "OK");
            }
            catch (Exception ex)
            {
                // Qualquer outro erro inesperado
                await DisplayAlert("Ops", ex.Message, "OK!");
            }
        }
    }
}