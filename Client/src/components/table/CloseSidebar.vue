<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { PanelLeftCloseIcon, PanelLeftOpenIcon } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();

const openMenu = computed(() => store.openMenu);
</script>

<template>
  <Button
    variant="outline"
    class="size-8"
    size="icon"
    @click="store.setOpenMenu(!openMenu)"
  >
    <PanelLeftCloseIcon :size="16" v-if="openMenu" />
    <PanelLeftOpenIcon :size="16" v-else />
  </Button>
</template>
