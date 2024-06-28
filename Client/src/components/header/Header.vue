<script setup lang="ts">
import Logo from "@/components/header/Logo.vue";
import OpenTable from "@/components/header/OpenTable.vue";
import OpenTablesList from "@/components/header/OpenTableList.vue";
import RefreshButton from "@/components/header/Refresh.vue";
import SettingsButton from "@/components/header/Settings.vue";
</script>

<template>
  <div class="mx-auto flex h-[50px] items-center gap-2 border-b-[1px] px-4">
    <Logo />
    <RefreshButton />
    <OpenTable />
    <OpenTablesList />
    <SettingsButton />
  </div>
</template>
