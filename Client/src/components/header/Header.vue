<script setup lang="ts">
import Logo from "@/components/header/Logo.vue";
import OpenTables from "@/components/header/OpenTables.vue";
import RefreshButton from "@/components/header/RefreshButton.vue";
import SettingsButton from "@/components/header/SettingsButton.vue";
</script>

<template>
  <header class="mx-auto flex h-[50px] items-center gap-2 border-b-[1px] px-4">
    <Logo />
    <RefreshButton />
    <OpenTables />
    <SettingsButton />
  </header>
</template>
