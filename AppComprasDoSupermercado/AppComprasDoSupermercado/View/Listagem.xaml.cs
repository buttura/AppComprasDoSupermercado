﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppComprasDoSupermercado.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace AppComprasDoSupermercado.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listagem : ContentPage
    {
        ObservableCollection<Produto> lista_produtos = new ObservableCollection<Produto>();
        public Listagem()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista_produtos;
        }

        private void ToolbarItem_Clicked_Novo(object sender, EventArgs e)
        {
            try {
                Navigation.PushAsync(new NovoProduto());
            } catch (Exception ex)
            {
                DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void ToolbarItem_Clicked_Somar(object sender, EventArgs e)
        {
            double soma = lista_produtos.Sum(i => i.Preco * i.Quantidade);
            string msg = "O total da compra é: " + soma;

            DisplayAlert("Ops", msg, "OK");
        }

        protected override void OnAppearing()
        {
            if (lista_produtos.Count == 0)
            {
                System.Threading.Tasks.Task.Run(async () =>
                {
                    List<Produto> temp = await App.Database.getAll();

                    foreach (Produto item in temp)
                    {
                        lista_produtos.Add(item);
                    }
                    ref_carregando.IsRefreshing = false;
                });

            }
        }

        private void txt_busca_TextChanged(object sender, TextChangedEventArgs e)
        {
            string buscou = e.NewTextValue;
            System.Threading.Tasks.Task.Run(async () =>
            {
                List<Produto> temp = await App.Database.Search(buscou);
                lista_produtos.Clear();

                foreach (Produto item in temp)
                {
                    lista_produtos.Add(item);
                }
                ref_carregando.IsRefreshing = false;
            });
        }

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Produto produto_selecionado = (Produto)e.SelectedItem;
            EditarProduto pagina = new EditarProduto
            {
                BindingContext = produto_selecionado
            };
            Navigation.PushAsync(pagina);
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            MenuItem disparador = (MenuItem)sender;
            Produto produto_selecionado = (Produto)disparador.BindingContext;
            bool confirmacao = await DisplayAlert("Tem Certeza?", "Remover Item?", "Sim", "Não");
            if (confirmacao)
            {
                await App.Database.Delete(produto_selecionado.Id);
                lista_produtos.Remove(produto_selecionado);
            }
        }
    }
}