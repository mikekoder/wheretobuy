<template>
  <q-page class="q-px-sm">
    <div class="row q-mt-sm">
      <div class="col">
        <q-input v-model="text" float-label="Tuote"></q-input>
      </div>
    </div>
    <div class="row q-mt-md">
      <div class="col">
        <q-input v-model="location" float-label="Paikkakunta"></q-input>
      </div>
    </div>
    <div class="row q-my-md">
      <div class="col">
        <q-btn color="primary" label="Etsi" @click="search" :disabled="!canSearch"></q-btn>
      </div>
    </div>
    <q-tabs v-model="tab" v-if="showData">
        <q-tab slot="title" name="tab-1" label="Kaupoittain" />
        <q-tab slot="title" name="tab-2" label="Tuotteittain" />

        <q-tab-pane name="tab-1">
          <q-card v-for="store in byStore">
          <q-card-title class=" bg-grey-4">
            {{ store.storeName }}
          </q-card-title>
          <q-card-separator />
          <q-card-main>
            <div class="row" v-for="product in store.products">
              <div class="col-8">
                {{ product.name }}
              </div>
              <div class="col-4">
                {{ product.price }} €
              </div>
            </div>
          </q-card-main>
          </q-card>
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <q-card v-for="product in byProduct">
          <q-card-title class=" bg-grey-4">
            {{ product.name }}
          </q-card-title>
          <q-card-separator />
          <q-card-main>
            <div class="row" v-for="store in product.stores">
              <div class="col-8">
                {{ store.storeName }}
              </div>
              <div class="col-4">
                {{ store.price }} €
              </div>
            </div>
          </q-card-main>
          </q-card>
        </q-tab-pane>
    </q-tabs>
    <div class="row" v-if="showLoader">
      <div class="col">
        Haetaan tietoja...<br />
        Taustalla tehdään paljon hakuja, joten tässä voi kestää useita kymmeniä sekunteja.
      </div>
    </div>
    <div v-if="!showData" class="q-mt-lg">
      Haut tehdään K-Ruoka- ja Foodie.fi-palveluihin, joten tuotteet rajoittuvat lähinnä elintarvikkeisiin ja päivittäistavaraan.
    </div>
  </q-page>
</template>

<style>
</style>

<script>
export default {
  name: 'PageIndex',
  data(){
    return {
      text: '',
      location: '',
      tab: 'tab-1',
      byStore: [],
      byProduct: [],
      loading: false,
      searched: false
    }
  },
  computed:{
    canSearch(){
      return this.text && this.location;
    },
    showData(){
      return !this.loading && this.searched;
    },
    showLoader(){
      return this.loading;
    }
  },
  methods:{
    search(){
      this.loading = true;
      this.searched = true;
      this.byStore = [];
      this.byProduct = [];
      this.$axios.get(`http://api.wheretobuy.mikakolari.fi/api/search?text=${this.text}&location=${this.location}`)
      .then(response => {

        var byStore = response.data;
        var byProduct = [];

        byStore.forEach(s => {
          s.products.forEach(p => {
            var product = byProduct.find(p2 => p2.ean == p.ean);
            if(!product){
              product = { ean: p.ean, name: p.name, stores: []};
              byProduct.push(product);
            }
            else if(product.name.length < p.name.length){
              product.name = p.name;
            }

            product.stores.push({ storeName: s.storeName, price: p.price});

          });
        });

        byStore.forEach(s => {
          s.products = s.products.sort((a,b) =>  a.name < b.name ? -1 : 1);
        });
        byProduct.forEach(p => {
          p.stores = p.stores.sort((a,b) =>  a.price < b.price ? -1 : 1);
        });

        this.byStore = byStore.sort((a,b) => a.storeName < b.storeName ? -1 : 1);
        this.byProduct = byProduct.sort((a,b) => a.name < b.name ? -1 : 1);
        this.loading = false;
      }).catch(() => {
        this.loading = false;
      });
    }
  }
}
</script>
