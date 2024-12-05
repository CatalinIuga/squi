<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { RefreshCw } from "lucide-vue-next";

const store = useSquiStore();
</script>

<template>
  <Button
    @click="store.refreshTableData"
    class="flex items-center gap-1 size-8"
    variant="outline"
    size="icon"
  >
    <RefreshCw :size="16" />
  </Button>
</template>
