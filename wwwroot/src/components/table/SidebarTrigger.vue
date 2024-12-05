<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { PanelLeftCloseIcon, PanelLeftOpenIcon } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();

const showSidebar = computed(() => store.showSidebar);
</script>

<template>
  <Button
    variant="outline"
    class="size-8"
    size="icon"
    @click="store.setShowSidebar(!showSidebar)"
  >
    <PanelLeftCloseIcon :size="17" v-if="showSidebar" />
    <PanelLeftOpenIcon :size="17" v-else />
  </Button>
</template>
