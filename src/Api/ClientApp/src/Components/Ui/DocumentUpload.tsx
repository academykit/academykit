import { useState } from 'react';
import { FilePond, registerPlugin } from 'react-filepond';
import 'filepond/dist/filepond.min.css';
import FilePondPluginImageExifOrientation from 'filepond-plugin-image-exif-orientation';
import FilePondPluginImagePreview from 'filepond-plugin-image-preview';
import { Box } from '@mantine/core';
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import FilePondPluginImageValidateSize from 'filepond-plugin-image-validate-size';
import { useTranslation } from 'react-i18next';

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginImageValidateSize
);

const DcoumentUpload = ({ setUrl }: { setUrl: (url: string) => void }) => {
  const [files, setFiles] = useState<any>([]);
  const { t } = useTranslation();
  const filePondProps = {
    labelInvalidField: t('Field contains invalid files'),
    labelFileWaitingForSize: t('Waiting for size'),
    labelFileSizeNotAvailable: t('Size not available'),
    labelFileLoading: t('Loading'),
    labelFileLoadError: t('Error during load'),
    labelFileProcessing: t('Processing'),
    labelFileProcessingComplete: t('Processing complete'),
    labelFileProcessingAborted: t('Processing aborted'),
    labelFileProcessingError: t('Error during processing'),
    labelFileProcessingRevertError: t('Error during revert'),
    labelFileRemoveError: t('Error during removal'),
    labelTapToCancel: t('Tap to cancel'),
    labelTapToRetry: t('Tap to retry'),
    labelTapToUndo: t('Tap to undo'),
    labelButtonRemoveItem: t('Remove'),
    labelButtonAbortItemLoad: t('Abort'),
    labelButtonRetryItemLoad: t('Retry'),
    labelButtonAbortItemProcessing: t('Abort'),
    labelButtonUndoItemProcessing: t('Undo'),
    labelButtonRetryItemProcessing: t('Retry'),
    labelButtonProcessItem: t('Process'),
  };
  return (
    <Box my={10} sx={{ maxWidth: 470 }}>
      <FilePond
        files={files}
        onaddfile={(error) => {
          if (!error) {
            setUrl('hello');
          }
        }}
        acceptedFileTypes={['application/pdf']}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        // server="/api"
        name="files"
        labelIdle={`${t(
          'drag_document'
        )}<span class="filepond--label-action">${t('browse')}</span>`}
        {...filePondProps}
      />
    </Box>
  );
};

export default DcoumentUpload;
