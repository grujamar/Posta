<script>
window.onbeforeunload = function (example) {
   example = example || window.event;
   // For IE and Firefox prior to version 4
   if (example) {
      example.returnValue = 'Are you sure you want to close the Tab?';
   }
   // For Safari
   return 'Are you sure you want to close the Tab?';
};
</script>