<script setup lang="ts">
import OpenSqlEditor from "@/components/sidebar/OpenSqlEditor.vue";
import TableList from "@/components/sidebar/TableList.vue";

import { useSquiStore } from "@/lib/store";
import { computed } from "vue";

const store = useSquiStore();
const openMenu = computed(() => store.openMenu);
</script>

<template>
  <aside
    v-show="openMenu"
    class="flex min-w-[0] max-w-[20%] grow flex-col border border-border bg-background border-y-0 border-l-0"
  >
    <OpenSqlEditor />
    <TableList />
  </aside>
</template>
