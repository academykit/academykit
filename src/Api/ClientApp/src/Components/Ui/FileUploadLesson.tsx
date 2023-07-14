import { useEffect, useState } from 'react';
import { FilePond, registerPlugin } from 'react-filepond';
import 'filepond/dist/filepond.min.css';
import FilePondPluginImageExifOrientation from 'filepond-plugin-image-exif-orientation';
import FilePondPluginImagePreview from 'filepond-plugin-image-preview';
import { Box, Text } from '@mantine/core';
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import FilePondPluginImageValidateSize from 'filepond-plugin-image-validate-size';
import { FileAccess, uploadFile } from '@utils/services/fileService';
import { UseFormReturnType } from '@mantine/form';
import { useTranslation } from 'react-i18next';

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginImageValidateSize
);

const FileUploadLesson = ({
  currentFile,
  formContext,
}: {
  currentFile?: string;
  formContext: () => UseFormReturnType<any, (values: any) => any>;
}) => {
  const { t } = useTranslation();

  useEffect(() => {
    if (currentFile) {
      setFiles([
        {
          source: currentFile,
          options: {
            type: 'local',
          },
        },
      ]);
    }
  }, [currentFile]);

  const [files, setFiles] = useState<any>([]);
  const form = formContext();
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
    <Box sx={{ maxWidth: 470 }} pos="relative">
      <FilePond
        instantUpload={true}
        files={files}
        labelIdle={`${t(
          'drag_and_drop_file'
        )} <span class="filepond--label-action">${t('browse')}</span>`}
        onaddfile={() => {}}
        onremovefile={() => form.setFieldValue('documentUrl', '')}
        onupdatefiles={setFiles}
        acceptedFileTypes={['application/pdf']}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (
            fieldName,
            file,
            metadata,
            load,
            error,
            progress,
            abort
          ) => {
            try {
              const res = await uploadFile(file as File, FileAccess.Private);
              load(res.data);
              form.setFieldValue('documentUrl', res.data);
            } catch (e) {
              error('Unable to upload file');
            }
            return {
              abort: () => {
                abort();
              },
            };
          },
          load: async (source, load, error, _progress, abort) => {
            await fetch(
              `${source}?cache=${Math.random().toString(36).substring(2, 7)}`
            )
              .then(async (r) => {
                load(await r.blob());
              })
              .catch((r) => error(r));
            // Should expose an abort method so the request can be cancelled
            return {
              abort: () => {
                abort();
              },
            };
          },
        }}
        {...filePondProps}
      />
      {form.errors['documentUrl'] && (
        <Text color={'red'} size={'xs'} pos="absolute" top={'100%'}>
          {form.errors['documentUrl']}
        </Text>
      )}
    </Box>
  );
};

export default FileUploadLesson;
