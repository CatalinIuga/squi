import { defineStore } from "pinia";
import { ref } from "vue";

export const useSquiStore = defineStore("squi", () => {
  const tableName = ref<string | null>(localStorage.getItem("tableName"));
});
