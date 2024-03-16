<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useColorMode } from "@vueuse/core";
import { MoonIcon, SunIcon } from "lucide-vue-next";

const mode = useColorMode();

const toggleColorMode = () => {
  mode.value = mode.value === "dark" ? "light" : "dark";
};
</script>

<template>
  <Button
    @click="toggleColorMode"
    class="flex items-center gap-2 px-2"
    variant="outline"
    size="sm"
  >
    <SunIcon v-if="mode === 'light'" :size="16" />
    <MoonIcon v-else :size="16" />
  </Button>
</template>
