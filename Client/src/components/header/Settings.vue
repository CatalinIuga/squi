<script setup lang="ts">
import ThemeSwitcher from "@/components/header/ThemeSwitcher.vue";
import { Button } from "@/components/ui/button";
import { DownloadIcon } from "lucide-vue-next";
</script>

<template>
  <div class="ml-auto gap-2 flex items-center">
    <Button
      class="flex items-center gap-1 size-8"
      variant="outline"
      size="icon"
    >
      <DownloadIcon :size="16" />
    </Button>
    <ThemeSwitcher />
  </div>
</template>
