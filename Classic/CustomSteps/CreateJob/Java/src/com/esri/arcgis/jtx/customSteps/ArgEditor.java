/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.?*/
package com.esri.arcgis.jtx.customSteps;

import com.esri.arcgis.jtx.IJTXConfiguration2;
import com.esri.arcgis.jtx.IJTXDatabase;
import com.esri.arcgis.jtx.IJTXJobType;
import com.esri.arcgis.jtx.IJTXJobTypeSet;
import com.esri.arcgis.jtx.IJTXUserGroupSet;
import com.esri.arcgis.jtx.IJTXUserSet;
import com.esri.arcgis.jtx.utilities.StepUtilities;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import javax.swing.JDialog;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

/**
 * Argument editor for the create job custom step.  I will allow the user to
 * choose what job type to create, and what user or group they want the created
 * job to be assigned to
 *
 */
public class ArgEditor extends javax.swing.JDialog {

    private IJTXDatabase m_ipDatabase = null;
    private String[] m_expectedArgs;
    private List<String> m_Arguments = null;
    private boolean dialogResult;
    

    /** Creates new form ArgEditor */
    public ArgEditor() {
        initComponents();
    }

    /**
     * Contstructor to set the database and expected arguments
     * @param parent the parent dialog
     * @param ipDB the database
     * @param expectedArgs the expected arguments
     */
    public ArgEditor(JDialog parent, IJTXDatabase ipDB, String[] expectedArgs) {
        super (parent, true);

        try {
            // Try to set the look and feel, and ignore any exceptions
            UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
        } catch (IllegalAccessException ex) {
        } catch (UnsupportedLookAndFeelException ex) {
        } catch (ClassNotFoundException ex) {
        } catch (InstantiationException ex) {}

        initComponents();

        m_ipDatabase = ipDB;
        m_expectedArgs = expectedArgs;
        m_Arguments = new ArrayList<String>();
    }

    /**
     * Show the argument editor dialog
     * @param argsIn the arguments that have previously been configured
     * @return String[] the updated arguments
     */
    public String [] showDialog(String[] argsIn) {
        // Populate the combo boxes with the appropriate information
        IJTXConfiguration2 ipJTXConfig = null;
        try {
            ipJTXConfig = (IJTXConfiguration2) m_ipDatabase.getConfigurationManager();

            populateJobTypes(ipJTXConfig);
            populateUsers(ipJTXConfig);
            populateGroups(ipJTXConfig);

            // Populate the dialog with the existing argument information
            String strTemp = StepUtilities.getArgument(argsIn, m_expectedArgs[0], true);
            if (strTemp != null) {
                // Then the job type has been entered
                int iJobTypeID = Integer.parseInt(strTemp);
                IJTXJobType ipJobType = ipJTXConfig.getJobTypeByID(iJobTypeID);

                cmbJobTypes.setSelectedItem(ipJobType.getName());
            }

            strTemp = StepUtilities.getArgument(argsIn, m_expectedArgs[1], true);
            if (strTemp != null) {
                // Then a user group has been selected for the new job assignment
                chkGroup.setSelected(true);
                chkUser.setSelected(false);
                cmbGroups.setEnabled(true);
                cmbGroups.setSelectedItem(strTemp);
            }
            strTemp = StepUtilities.getArgument(argsIn, m_expectedArgs[2], true);
            if (strTemp != null) {
                // Then a user has been selected for the new job assignment
                chkGroup.setSelected(false);
                chkUser.setSelected(true);
                cmbUsers.setEnabled(true);
                cmbUsers.setSelectedItem(strTemp);
            }

            // Show the dialog
            m_ipDatabase.logMessage(5, 1000, "About to show the dialog");
            this.setVisible(true);
            m_ipDatabase.logMessage(5, 1000, "Dialog was closed");

            if (dialogResult) {
                m_ipDatabase.logMessage(5, 1000, "OK, clicked... Building arguments");
                if (!cmbJobTypes.getSelectedItem().toString().equals("")) {
                    IJTXJobType ipJobType = ipJTXConfig.getJobType(cmbJobTypes.getSelectedItem().toString());
                    m_Arguments.add(StepUtilities.createSingleArgument(m_expectedArgs[0], String.valueOf(ipJobType.getID())));
                }

                if (chkGroup.isSelected()) {
                    m_Arguments.add(StepUtilities.createSingleArgument(m_expectedArgs[1], cmbGroups.getSelectedItem().toString()));
                } else if (chkUser.isSelected()) {
                    m_Arguments.add(StepUtilities.createSingleArgument(m_expectedArgs[2], cmbUsers.getSelectedItem().toString()));
                }
                m_ipDatabase.logMessage(5, 1000, m_Arguments.size() + " arguments built");

                m_ipDatabase.logMessage(5, 1000, "There are " + m_Arguments.size() + " items");
                return m_Arguments.toArray(new String[0]);
            } else {
                return argsIn;
            }
        } catch (IOException ex) {
            try {
                m_ipDatabase.logMessage(3, 500, ex.getMessage());
            } catch (IOException ex1) {
            }
            return argsIn;
        }
    }

    /** This method is called from within the constructor to
     * initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is
     * always regenerated by the Form Editor.
     */
    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        jLabel1 = new javax.swing.JLabel();
        cmbJobTypes = new javax.swing.JComboBox();
        chkGroup = new javax.swing.JCheckBox();
        cmbGroups = new javax.swing.JComboBox();
        chkUser = new javax.swing.JCheckBox();
        cmbUsers = new javax.swing.JComboBox();
        cmdOK = new javax.swing.JButton();
        cmdCancel = new javax.swing.JButton();

        setTitle("Create Job Arguments");
        setAlwaysOnTop(true);
        setModalExclusionType(java.awt.Dialog.ModalExclusionType.APPLICATION_EXCLUDE);
        setResizable(false);

        jLabel1.setText("Select job type for new job:");

        chkGroup.setText("Assign new job to group:");
        chkGroup.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                chkGroupActionPerformed(evt);
            }
        });

        cmbGroups.setEnabled(false);

        chkUser.setText("Assign new job to user:");
        chkUser.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                chkUserActionPerformed(evt);
            }
        });

        cmbUsers.setEnabled(false);

        cmdOK.setText("OK");
        cmdOK.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                cmdOKActionPerformed(evt);
            }
        });

        cmdCancel.setText("Cancel");
        cmdCancel.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                cmdCancelActionPerformed(evt);
            }
        });

        javax.swing.GroupLayout layout = new javax.swing.GroupLayout(getContentPane());
        getContentPane().setLayout(layout);
        layout.setHorizontalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING, false)
                    .addComponent(jLabel1)
                    .addComponent(chkGroup)
                    .addComponent(chkUser)
                    .addGroup(javax.swing.GroupLayout.Alignment.TRAILING, layout.createSequentialGroup()
                        .addComponent(cmdOK)
                        .addGap(18, 18, 18)
                        .addComponent(cmdCancel))
                    .addComponent(cmbJobTypes, 0, 287, Short.MAX_VALUE)
                    .addComponent(cmbGroups, 0, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .addComponent(cmbUsers, 0, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap(javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );
        layout.setVerticalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addComponent(jLabel1)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.UNRELATED)
                .addComponent(cmbJobTypes, javax.swing.GroupLayout.PREFERRED_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addGap(18, 18, 18)
                .addComponent(chkGroup)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.UNRELATED)
                .addComponent(cmbGroups, javax.swing.GroupLayout.PREFERRED_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addGap(18, 18, 18)
                .addComponent(chkUser)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.UNRELATED)
                .addComponent(cmbUsers, javax.swing.GroupLayout.PREFERRED_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addGap(18, 18, 18)
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.BASELINE)
                    .addComponent(cmdOK)
                    .addComponent(cmdCancel))
                .addContainerGap(javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        pack();
    }// </editor-fold>//GEN-END:initComponents

    private void cmdCancelActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_cmdCancelActionPerformed
        dialogResult = false;
        this.setVisible(false);
    }//GEN-LAST:event_cmdCancelActionPerformed

    private void cmdOKActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_cmdOKActionPerformed
        dialogResult = true;
        this.setVisible(false);
    }//GEN-LAST:event_cmdOKActionPerformed

    private void chkGroupActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_chkGroupActionPerformed
        cmbGroups.setEnabled(chkGroup.isSelected());
        if (chkGroup.isSelected() & chkUser.isSelected()) {
            chkUser.setSelected(false);
            cmbUsers.setEnabled(false);
        }
    }//GEN-LAST:event_chkGroupActionPerformed

    private void chkUserActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_chkUserActionPerformed
        cmbUsers.setEnabled(chkUser.isSelected());
        if (chkGroup.isSelected() & chkUser.isSelected()) {
            chkGroup.setSelected(false);
            cmbGroups.setEnabled(false);
        }
    }//GEN-LAST:event_chkUserActionPerformed

    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JCheckBox chkGroup;
    private javax.swing.JCheckBox chkUser;
    private javax.swing.JComboBox cmbGroups;
    private javax.swing.JComboBox cmbJobTypes;
    private javax.swing.JComboBox cmbUsers;
    private javax.swing.JButton cmdCancel;
    private javax.swing.JButton cmdOK;
    private javax.swing.JLabel jLabel1;
    // End of variables declaration//GEN-END:variables

    private void populateJobTypes(IJTXConfiguration2 ipJTXConfig) throws IOException {
        cmbJobTypes.removeAllItems();
        IJTXJobTypeSet ipJobTypes = ipJTXConfig.getJobTypes();
        for (int i = 0; i < ipJobTypes.getCount(); i++) {
            cmbJobTypes.addItem(ipJobTypes.getItem(i).getName());
        }
        cmbJobTypes.setSelectedIndex(0);
    }

    private void populateUsers(IJTXConfiguration2 ipJTXConfig) throws IOException {
        cmbUsers.removeAllItems();
        IJTXUserSet ipUsers = ipJTXConfig.getUsers();
        for (int i = 0; i < ipUsers.getCount(); i++) {
            cmbUsers.addItem(ipUsers.getItem(i).getUserName());
        }
        cmbUsers.setSelectedIndex(0);
    }

    private void populateGroups(IJTXConfiguration2 ipJTXConfig) throws IOException {
        cmbGroups.removeAllItems();
        IJTXUserGroupSet ipGroups = ipJTXConfig.getUserGroups();
        for (int i = 0; i < ipGroups.getCount(); i++) {
            cmbGroups.addItem(ipGroups.getItem(i).getName());
        }
        cmbGroups.setSelectedIndex(0);
    }
}
