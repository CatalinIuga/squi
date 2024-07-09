<script setup lang="ts">
import Sidebar from "@/components/sidebar/Sidebar.vue";
import Table from "@/components/table/Table.vue";
import { Toaster } from "@/components/ui/toast";
</script>

<template>
  <main class="w-screen h-screen flex">
    <Sidebar />
    <Table />
  </main>
  <Toaster />
</template>
