import { createPinia } from "pinia";
import { createApp } from "vue";
import App from "./App.vue";
import "./styles.css";

const pinia = createPinia();
const app = createApp(App);
app.use(pinia);
app.mount("#app");
