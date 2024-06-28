<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { PlusIcon } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();
const openMenu = computed(() => store.openMenu);
</script>

<template>
  <Button
    @click="store.setOpenMenu(true)"
    :disabled="openMenu"
    class="size-8"
    variant="outline"
    size="icon"
  >
    <PlusIcon :size="16" />
  </Button>
</template>
