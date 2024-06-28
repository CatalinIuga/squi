<script setup lang="ts">
import { Button } from "@/components/ui/button";
</script>

<template>
  <Button class="flex text-sm" variant="default" size="sm">Add record</Button>
  <!-- Save Changes -->
  <Button
    class="flex text-sm bg-green-600 hover:bg-green-600/80"
    variant="default"
    size="sm"
  >
    Save
  </Button>
  <!-- Discard Changes -->
  <Button class="flex text-sm underline" variant="ghost" size="sm">
    Discard changes
  </Button>
  <!-- Delete records -->
  <Button class="flex text-sm" variant="destructive" size="sm">
    Delete records
  </Button>
</template>
